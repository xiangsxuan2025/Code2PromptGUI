using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Code2LlmPrompt.Views.Controls
{
    /// <summary>
    /// 自定义标题栏控件
    /// 提供窗口控制按钮和拖拽功能
    /// </summary>
    public partial class CustomTitleBar : UserControl
    {
        private Window? _hostWindow;  // 宿主窗口引用

        /// <summary>
        /// 构造函数
        /// 初始化组件并查找宿主窗口
        /// </summary>
        public CustomTitleBar()
        {
            InitializeComponent();

            // 查找宿主窗口
            this.AttachedToVisualTree += (s, e) =>
            {
                _hostWindow = this.FindAncestorOfType<Window>();
            };
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// 最小化宿主窗口
        /// </summary>
        private void MinimizeClick(object? sender, RoutedEventArgs e)
        {
            _hostWindow?.SetCurrentValue(Window.WindowStateProperty, WindowState.Minimized);
        }

        /// <summary>
        /// 最大化/还原按钮点击事件
        /// 切换宿主窗口的最大化状态
        /// </summary>
        private void MaximizeClick(object? sender, RoutedEventArgs e)
        {
            if (_hostWindow == null) return;

            if (_hostWindow.WindowState == WindowState.Maximized)
            {
                _hostWindow.WindowState = WindowState.Normal;
            }
            else
            {
                _hostWindow.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// 关闭宿主窗口
        /// </summary>
        private void CloseClick(object? sender, RoutedEventArgs e)
        {
            _hostWindow?.Close();
        }

        /// <summary>
        /// 指针按下事件处理
        /// 允许通过标题栏拖动窗口
        /// </summary>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && _hostWindow != null)
            {
                _hostWindow.BeginMoveDrag(e);
            }
            base.OnPointerPressed(e);
        }
    }
}
