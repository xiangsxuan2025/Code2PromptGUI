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
    /// 主窗口视图模型 - 核心结构部分
    /// 负责管理应用程序的主要业务逻辑和用户交互
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly ProcessRunner _processRunner;
        private Window? _mainWindow;

        /// <summary>
        /// 当前操作状态
        /// </summary>
        [ObservableProperty]
        private string _status = "Ready";

        /// <summary>
        /// 进程输出内容
        /// </summary>
        [ObservableProperty]
        private string _output = "";

        /// <summary>
        /// 生成的结果内容
        /// </summary>
        [ObservableProperty]
        private string _resultContent = "";

        /// <summary>
        /// 是否正在处理中
        /// </summary>
        [ObservableProperty]
        private bool _isProcessing;

        /// <summary>
        /// 工具状态信息
        /// </summary>
        [ObservableProperty]
        private string _toolStatus = "🔧 Tool: Ready";

        /// <summary>
        /// 是否启用高级模式
        /// </summary>
        [ObservableProperty]
        private bool _isAdvancedMode;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainViewModel()
        {
            _processRunner = new ProcessRunner();
            _processRunner.OutputReceived += OnOutputReceived;
            _processRunner.ErrorReceived += OnErrorReceived;
            _processRunner.ProcessExited += OnProcessExited;

            CheckToolAvailability();
        }

        /// <summary>
        /// 设置主窗口引用
        /// </summary>
        /// <param name="window">主窗口实例</param>
        public void SetMainWindow(Window window)
        {
            _mainWindow = window;
        }

        /// <summary>
        /// 检查工具可用性
        /// </summary>
        private void CheckToolAvailability()
        {
            var processRunner = new ProcessRunner();
            var toolPath = processRunner.GetType().GetField("_toolPath",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(processRunner) as string;

            if (!string.IsNullOrEmpty(toolPath) && File.Exists(toolPath))
            {
                ToolStatus = "🔧 Tool: Available";
            }
            else
            {
                ToolStatus = "🔧 Tool: Not Found";
            }
        }
    }
}
