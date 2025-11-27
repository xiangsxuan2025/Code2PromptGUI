using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code2LlmPrompt.ViewModels
{
    /// <summary>
    /// 主窗口视图模型 - 参数构建部分
    /// 负责构建命令行参数
    /// </summary>
    public partial class MainViewModel
    {/// <summary>
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
    }
}
