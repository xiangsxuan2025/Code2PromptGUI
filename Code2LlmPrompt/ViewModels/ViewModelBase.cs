using CommunityToolkit.Mvvm.ComponentModel;

namespace Code2LlmPrompt.ViewModels
{
    /// <summary>
    /// 视图模型基类
    /// 继承自ObservableObject，提供属性变更通知功能
    /// 所有视图模型都应从此类派生
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        // 基类目前没有额外功能，主要为未来的扩展预留
    }
}
