using Avalonia;
using Avalonia.Themes.Fluent;
using System;

namespace Code2LlmPrompt
{
    /// <summary>
    /// 应用程序入口点类
    /// 负责配置和启动Avalonia应用程序
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// 应用程序主入口点
        /// 使用经典桌面生命周期启动应用
        /// </summary>
        /// <param name="args">命令行参数</param>
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// 构建Avalonia应用程序
        /// 配置平台检测、字体和日志
        /// </summary>
        /// <returns>配置好的AppBuilder实例</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()  // 自动检测运行平台
                .WithInterFont()      // 使用Inter字体
                .LogToTrace();        // 启用跟踪日志
    }
}
