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
    /// 主窗口视图模型
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
        /// 要分析的代码路径
        /// </summary>
        [ObservableProperty]
        private string _path = ".";

        /// <summary>
        /// 输出文件名
        /// </summary>
        [ObservableProperty]
        private string _outputFileName = "code2prompt.txt";

        /// <summary>
        /// 是否复制到剪贴板
        /// </summary>
        [ObservableProperty]
        private bool _clipboard = false;

        /// <summary>
        /// 包含文件模式
        /// </summary>
        [ObservableProperty]
        private string _includePatterns = "";

        /// <summary>
        /// 排除文件模式
        /// </summary>
        [ObservableProperty]
        private string _excludePatterns = "";

        /// <summary>
        /// 是否跟随符号链接
        /// </summary>
        [ObservableProperty]
        private bool _followSymlinks;

        /// <summary>
        /// 是否包含隐藏文件
        /// </summary>
        [ObservableProperty]
        private bool _hidden;

        /// <summary>
        /// 是否忽略.gitignore规则
        /// </summary>
        [ObservableProperty]
        private bool _noIgnore;

        /// <summary>
        /// 输出格式
        /// </summary>
        [ObservableProperty]
        private string _outputFormat = "markdown";

        /// <summary>
        /// 模板文件路径
        /// </summary>
        [ObservableProperty]
        private string _template = "";

        /// <summary>
        /// 是否显示行号
        /// </summary>
        [ObservableProperty]
        private bool _lineNumbers;

        /// <summary>
        /// 是否使用绝对路径
        /// </summary>
        [ObservableProperty]
        private bool _absolutePaths;

        /// <summary>
        /// 是否禁用代码块
        /// </summary>
        [ObservableProperty]
        private bool _noCodeblock;

        /// <summary>
        /// 是否显示完整目录树
        /// </summary>
        [ObservableProperty]
        private bool _fullDirectoryTree;

        /// <summary>
        /// 是否包含Git差异
        /// </summary>
        [ObservableProperty]
        private bool _diff;

        /// <summary>
        /// Git差异分支
        /// </summary>
        [ObservableProperty]
        private string _gitDiffBranches = "";

        /// <summary>
        /// Git日志分支
        /// </summary>
        [ObservableProperty]
        private string _gitLogBranches = "";

        /// <summary>
        /// 编码方式
        /// </summary>
        [ObservableProperty]
        private string _encoding = "cl100k";

        /// <summary>
        /// Token格式
        /// </summary>
        [ObservableProperty]
        private string _tokenFormat = "format";

        /// <summary>
        /// 是否显示Token映射
        /// </summary>
        [ObservableProperty]
        private bool _tokenMap;

        /// <summary>
        /// 是否启用静默模式
        /// </summary>
        [ObservableProperty]
        private bool _quiet;

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
        /// 输出格式列表
        /// </summary>
        public ObservableCollection<string> OutputFormats { get; } = new()
        {
            "markdown", "json", "xml"
        };

        /// <summary>
        /// 编码方式列表
        /// </summary>
        public ObservableCollection<string> Encodings { get; } = new()
        {
            "cl100k", "p50k", "p50k_edit", "r50k"
        };

        /// <summary>
        /// Token格式列表
        /// </summary>
        public ObservableCollection<string> TokenFormats { get; } = new()
        {
            "raw", "format"
        };

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
        /// 切换高级模式命令
        /// </summary>
        [RelayCommand]
        private void ToggleAdvanced()
        {
            IsAdvancedMode = !IsAdvancedMode;

            if (_mainWindow != null)
            {
                if (IsAdvancedMode)
                {
                    _mainWindow.Width = 1200;
                    _mainWindow.Height = 800;
                }
                else
                {
                    _mainWindow.Width = 550;
                    _mainWindow.Height = 420;
                }
            }
        }

        /// <summary>
        /// 生成命令
        /// </summary>
        [RelayCommand]
        private async Task Generate()
        {
            if (IsProcessing) return;

            Output = "";
            ResultContent = "";
            IsProcessing = true;
            Status = "Generating prompt...";

            try
            {
                var arguments = BuildArguments();
                await _processRunner.RunProcessAsync(arguments);
            }
            catch (Exception ex)
            {
                Output = $"Error: {ex.Message}";
                Status = "Error";
                IsProcessing = false;
            }
        }

        /// <summary>
        /// 浏览路径命令
        /// </summary>
        [RelayCommand]
        private async Task BrowsePath()
        {
            var folder = await BrowseFolderAsync();
            if (folder != null)
            {
                Path = folder;
            }
        }

        /// <summary>
        /// 浏览输出文件命令
        /// </summary>
        [RelayCommand]
        private async Task BrowseOutput()
        {
            var file = await SaveFileAsync("Prompt output", new[] { "*.md", "*.txt", "*" });
            if (file != null)
            {
                OutputFileName = file;
            }
        }

        /// <summary>
        /// 浏览模板命令
        /// </summary>
        [RelayCommand]
        private async Task BrowseTemplate()
        {
            var file = await OpenFileAsync("Template files", new[] { "*.hbs", "*.md", "*.txt", "*" });
            if (file != null)
            {
                Template = file;
            }
        }

        /// <summary>
        /// 复制结果命令
        /// </summary>
        [RelayCommand]
        private async Task CopyResult()
        {
            if (string.IsNullOrEmpty(ResultContent)) return;

            try
            {
                if (_mainWindow?.Clipboard is { } clipboard)
                {
                    await clipboard.SetTextAsync(ResultContent);
                    Status = "Result copied to clipboard";
                }
                else
                {
                    Status = "Clipboard not available";
                }
            }
            catch (Exception ex)
            {
                Status = $"Copy failed: {ex.Message}";
            }
        }

        /// <summary>
        /// 保存结果命令
        /// </summary>
        [RelayCommand]
        private async Task SaveResult()
        {
            if (string.IsNullOrEmpty(ResultContent)) return;

            var file = await SaveFileAsync("Save result", new[] { "*.md", "*.txt", "*" });
            if (file != null)
            {
                try
                {
                    await File.WriteAllTextAsync(file, ResultContent);
                    Status = $"Result saved to {System.IO.Path.GetFileName(file)}";
                }
                catch (Exception ex)
                {
                    Status = $"Save failed: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// 构建命令行参数
        /// </summary>
        /// <returns>参数字符串</returns>
        private string BuildArguments()
        {
            var args = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(Path) && Path != ".")
                args.Append($" {Path}");

            args.Append($" -O {OutputFileName}");

            if (!string.IsNullOrEmpty(IncludePatterns))
            {
                foreach (var pattern in IncludePatterns.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(pattern))
                        args.Append($" -i {pattern.Trim()}");
                }
            }

            if (!string.IsNullOrEmpty(ExcludePatterns))
            {
                foreach (var pattern in ExcludePatterns.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(pattern))
                        args.Append($" -e {pattern.Trim()}");
                }
            }

            if (FollowSymlinks)
                args.Append(" -L");

            if (Hidden)
                args.Append(" --hidden");

            if (NoIgnore)
                args.Append(" --no-ignore");

            if (!string.IsNullOrEmpty(OutputFormat) && OutputFormat != "markdown")
                args.Append($" -F {OutputFormat}");

            if (!string.IsNullOrEmpty(Template))
                args.Append($" -t {Template}");

            if (LineNumbers)
                args.Append(" --line-numbers");

            if (AbsolutePaths)
                args.Append(" --absolute-paths");

            if (NoCodeblock)
                args.Append(" --no-codeblock");

            if (FullDirectoryTree)
                args.Append(" --full-directory-tree");

            if (Diff)
                args.Append(" --diff");

            if (!string.IsNullOrEmpty(GitDiffBranches))
            {
                var branches = GitDiffBranches.Split(',');
                if (branches.Length == 2)
                    args.Append($" --git-diff-branch {branches[0].Trim()},{branches[1].Trim()}");
            }

            if (!string.IsNullOrEmpty(GitLogBranches))
            {
                var branches = GitLogBranches.Split(',');
                if (branches.Length == 2)
                    args.Append($" --git-log-branch {branches[0].Trim()},{branches[1].Trim()}");
            }

            if (!string.IsNullOrEmpty(Encoding) && Encoding != "cl100k")
                args.Append($" --encoding {Encoding}");

            if (!string.IsNullOrEmpty(TokenFormat) && TokenFormat != "format")
                args.Append($" --token-format {TokenFormat}");

            if (TokenMap)
                args.Append(" --token-map");

            if (Quiet)
                args.Append(" -q");

            return args.ToString().Trim();
        }

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

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <returns>文件夹路径</returns>
        private async Task<string?> BrowseFolderAsync()
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null) return null;

            var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select directory to analyze",
                AllowMultiple = false
            });

            return folders.Count > 0 ? folders[0].Path.LocalPath : null;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="fileTypes">文件类型</param>
        /// <returns>文件路径</returns>
        private async Task<string?> OpenFileAsync(string title, string[] fileTypes)
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null) return null;

            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = fileTypes.Select(ft =>
                    new FilePickerFileType(System.IO.Path.GetExtension(ft).TrimStart('.').ToUpper() + " Files")
                    {
                        Patterns = new[] { ft }
                    }).ToArray()
            });

            return files.Count > 0 ? files[0].Path.LocalPath : null;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="fileTypes">文件类型</param>
        /// <returns>文件路径</returns>
        private async Task<string?> SaveFileAsync(string title, string[] fileTypes)
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null) return null;

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                FileTypeChoices = fileTypes.Select(ft =>
                    new FilePickerFileType(System.IO.Path.GetExtension(ft).TrimStart('.').ToUpper() + " Files")
                    {
                        Patterns = new[] { ft }
                    }).ToArray()
            });

            return file?.Path.LocalPath;
        }

        /// <summary>
        /// 获取存储提供者
        /// </summary>
        /// <returns>存储提供者实例</returns>
        private IStorageProvider? GetStorageProvider()
        {
            return TopLevel.GetTopLevel(_mainWindow)?.StorageProvider;
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
