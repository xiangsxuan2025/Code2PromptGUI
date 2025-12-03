using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Code2LlmPrompt.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Code2LlmPrompt.ViewModels
{
    /// <summary>
    /// 主窗口视图模型 - 事件处理部分
    /// 处理进程运行器的事件回调
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 输出接收事件处理
        /// </summary>
        /// <param name="data">输出数据</param>
        private void OnOutputReceived(string data)
        {
            Output += data + Environment.NewLine;
            Debug.WriteLine(data);
        }

        /// <summary>
        /// 错误接收事件处理
        /// </summary>
        /// <param name="data">错误数据</param>
        private void OnErrorReceived(string data)
        {
            Output += $"ERROR: {data}{Environment.NewLine}";
            Debug.WriteLine(data);
        }

        /// <summary>
        /// 进程退出事件处理
        /// </summary>
        /// <param name="exitCode">退出代码</param>
        private async void OnProcessExited(int exitCode)
        {
            IsProcessing = false;
            Status = exitCode == 0 ? "Completed" : "Failed";
            // 哪怕只有1M的文件,也会导致内存增长为300M
            // todo: 这里可以排查一下底层原因
            GC.Collect();
            // 最终尝试读取输出文件，使用流式读取避免内存问题
            if (exitCode == 0 && File.Exists(OutputFileName))
            {
                try
                {
                    // 哪怕只有1M的文件,也会导致内存增长为300M
                    // todo: 这里可以排查一下底层原因
                    // 使用流式读取，限制最大文件大小
                    const long maxFileSizeMB = 1; // 1MB限制
                    const long maxFileSize = maxFileSizeMB * 1024 * 1024; // 1MB限制
                    var fileInfo = new FileInfo(OutputFileName);

                    if (fileInfo.Length > maxFileSize)
                    {
                        // 大文件只读取部分内容并提示
                        ResultContent = await ReadFileWithLimitAsync(OutputFileName, maxFileSize);
                        Status = $"Completed - File too large, showing first {maxFileSizeMB}MB";
                    }
                    else
                    {
                        // 小文件正常读取
                        ResultContent = await File.ReadAllTextAsync(OutputFileName);
                        Status = "Completed - Result ready";
                    }
                }
                catch (Exception ex)
                {
                    Output += $"Error reading output file: {ex.Message}{Environment.NewLine}";
                }
            }
        }
        /// <summary>
        /// 限制大小的文件读取方法
        /// </summary>
        private async Task<string> ReadFileWithLimitAsync(string filePath, long maxSize)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // 限制读取大小
            var sizeToRead = Math.Min(maxSize, fileStream.Length);
            var buffer = new byte[sizeToRead];

            await fileStream.ReadAsync(buffer, 0, (int)sizeToRead);

            var result = System.Text.Encoding.UTF8.GetString(buffer);

            if (fileStream.Length > maxSize)
            {
                result += $"\n\n[File truncated. Total size: {FormatFileSize(fileStream.Length)}]";
            }

            return result;

            /// 格式化文件大小为可读字符串
            string FormatFileSize(long bytes)
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = bytes;
                int order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len /= 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }
        }
    }

}

