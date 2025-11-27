using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Code2LlmPrompt.ViewModels;
using Code2LlmPrompt.Views;
using System.Linq;

namespace Code2LlmPrompt
{
    public partial class App : Application
    {
        /// <summary>
        /// 应用程序初始化方法
        /// 加载XAML资源
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 框架初始化完成后的处理
        /// 设置主窗口和数据上下文
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 避免Avalonia和CommunityToolkit的重复验证
                // 更多信息: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                // 创建主窗口和视图模型
                var mainWindow = new MainWindow();
                var viewModel = new MainViewModel();

                // 设置数据上下文并传递窗口引用
                mainWindow.DataContext = viewModel;
                viewModel.SetMainWindow(mainWindow);

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// 禁用Avalonia的数据注解验证
        /// 避免与CommunityToolkit的验证冲突
        /// </summary>
        private void DisableAvaloniaDataAnnotationValidation()
        {
            // 获取要移除的验证插件数组
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // 移除每个找到的验证插件
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}
