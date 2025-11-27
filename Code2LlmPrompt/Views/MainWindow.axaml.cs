using Avalonia.Controls;
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
    }
}
