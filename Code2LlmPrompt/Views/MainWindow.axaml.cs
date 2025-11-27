using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Code2LlmPrompt.ViewModels;

namespace Code2LlmPrompt.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 设置视图模型并传递窗口引用
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SetMainWindow(this);
            }
        }

        // 最小化按钮点击事件
        private void MinimizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // 最大化/还原按钮点击事件
        private void MaximizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        // 关闭按钮点击事件
        private void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }

        // 允许通过标题栏拖动窗口
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
            base.OnPointerPressed(e);
        }
    }
}
