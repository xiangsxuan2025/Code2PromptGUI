using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Code2LlmPrompt.Models
{
    /// <summary>
    /// 进程运行器类
    /// 负责执行外部code2prompt工具并处理其输出
    /// </summary>
    public class ProcessRunner
    {
        // 事件定义
        public event Action<string>? OutputReceived;  // 输出数据接收事件

        public event Action<string>? ErrorReceived;   // 错误数据接收事件

        public event Action<int>? ProcessExited;      // 进程退出事件

        private readonly string _toolPath;  // 工具路径

        /// <summary>
        /// 构造函数
        /// 初始化工具路径
        /// </summary>
        public ProcessRunner()
        {
            // 获取内置工具的路径
            _toolPath = GetEmbeddedToolPath();
        }

        /// <summary>
        /// 异步运行外部进程
        /// 执行code2prompt工具并处理输出
        /// </summary>
        /// <param name="arguments">命令行参数</param>
        /// <returns>异步任务</returns>
        public async Task RunProcessAsync(string arguments)
        {
            // 检查工具是否存在
            if (!File.Exists(_toolPath))
            {
                ErrorReceived?.Invoke($"Tool not found: {_toolPath}");
                ProcessExited?.Invoke(1);
                return;
            }

            // 配置进程启动信息
            var processStartInfo = new ProcessStartInfo
            {
                FileName = _toolPath,           // 可执行文件路径
                Arguments = arguments,          // 命令行参数
                RedirectStandardOutput = true,  // 重定向标准输出
                RedirectStandardError = true,   // 重定向错误输出
                UseShellExecute = false,        // 不使用操作系统shell
                CreateNoWindow = true,          // 不创建窗口
                WorkingDirectory = Environment.CurrentDirectory,  // 工作目录
                // 设置环境变量以避免某些问题
                Environment =
                {
                    ["RUST_BACKTRACE"] = "1" // 启用详细的错误回溯
                }
            };

            using var process = new Process { StartInfo = processStartInfo };

            // 注册输出数据接收事件
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    OutputReceived?.Invoke(e.Data);
                }
            };

            // 注册错误数据接收事件
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    ErrorReceived?.Invoke(e.Data);
                }
            };

            // 启用进程事件
            process.EnableRaisingEvents = true;

            // 注册进程退出事件
            process.Exited += (sender, e) =>
            {
                ProcessExited?.Invoke(process.ExitCode);
            };

            try
            {
                // 启动进程并开始异步读取输出
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // 等待进程退出
                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                // 处理进程执行异常
                ErrorReceived?.Invoke($"Process error: {ex.Message}");
                ProcessExited?.Invoke(1);
            }
        }

        /// <summary>
        /// 获取嵌入式工具路径
        /// 尝试多个可能的路径位置
        /// </summary>
        /// <returns>工具文件路径</returns>
        private string GetEmbeddedToolPath()
        {
            // 尝试多种可能的路径
            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Tools", "code2prompt.exe"),
                Path.Combine(AppContext.BaseDirectory, "code2prompt.exe"),
                Path.Combine(Directory.GetCurrentDirectory(), "Tools", "code2prompt.exe"),
                Path.Combine(Directory.GetCurrentDirectory(), "code2prompt.exe")
            };

            // 遍历所有可能的路径，返回第一个存在的路径
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // 如果找不到，返回第一个路径（将在运行时产生错误）
            return possiblePaths[0];
        }
    }
}
