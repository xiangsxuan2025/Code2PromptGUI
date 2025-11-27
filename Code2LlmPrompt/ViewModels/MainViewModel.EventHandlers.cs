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

            // 如果输出文件存在，读取其内容到ResultContent
            // todo 文件很大的话, 可能有内存问题
            if (File.Exists(OutputFileName))
            {
                try
                {
                    ResultContent = File.ReadAllText(OutputFileName);
                }
                catch (Exception ex)
                {
                    Output += $"Error reading output file: {ex.Message}{Environment.NewLine}";
                }
            }
        }

        /// <summary>
        /// 错误接收事件处理
        /// </summary>
        /// <param name="data">错误数据</param>
        private void OnErrorReceived(string data)
        {
            Output += $"ERROR: {data}{Environment.NewLine}";
        }

        /// <summary>
        /// 进程退出事件处理
        /// </summary>
        /// <param name="exitCode">退出代码</param>
        private void OnProcessExited(int exitCode)
        {
            IsProcessing = false;
            Status = exitCode == 0 ? "Completed" : "Failed";

            // 最终尝试读取输出文件
            // todo 文件很大的话, 可能有内存问题
            if (exitCode == 0 && File.Exists(OutputFileName))
            {
                try
                {
                    ResultContent = File.ReadAllText(OutputFileName);
                    Status = "Completed - Result ready";
                }
                catch (Exception ex)
                {
                    Output += $"Error reading output file: {ex.Message}{Environment.NewLine}";
                }
            }
        }
    }
}
