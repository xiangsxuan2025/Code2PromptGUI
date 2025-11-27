using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Code2LlmPrompt.Views.Controls
{
    public partial class CustomTitleBar : UserControl
    {
        private Window? _hostWindow;

        public CustomTitleBar()
        {
            InitializeComponent();

            // 查找宿主窗口
            this.AttachedToVisualTree += (s, e) =>
            {
                _hostWindow = this.FindAncestorOfType<Window>();
            };
        }

        // 最小化按钮点击事件
        private void MinimizeClick(object? sender, RoutedEventArgs e)
        {
            _hostWindow?.SetCurrentValue(Window.WindowStateProperty, WindowState.Minimized);
        }

        // 最大化/还原按钮点击事件
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

        // 关闭按钮点击事件
        private void CloseClick(object? sender, RoutedEventArgs e)
        {
            _hostWindow?.Close();
        }

        // 允许通过标题栏拖动窗口
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
