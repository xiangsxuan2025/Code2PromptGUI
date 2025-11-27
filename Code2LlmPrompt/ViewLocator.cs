using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Code2LlmPrompt.ViewModels;

namespace Code2LlmPrompt
{
    /// <summary>
    /// 视图定位器类
    /// 根据视图模型自动查找并创建对应的视图
    /// 实现ViewModel到View的自动映射
    /// </summary>
    [RequiresUnreferencedCode(
        "Default implementation of ViewLocator involves reflection which may be trimmed away.",
        Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// 构建视图
        /// 根据视图模型类型创建对应的视图实例
        /// </summary>
        /// <param name="param">视图模型实例</param>
        /// <returns>对应的视图控件</returns>
        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            // 将ViewModel类名替换为View类名
            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            // 如果找不到对应的视图，返回提示文本
            return new TextBlock { Text = "Not Found: " + name };
        }

        /// <summary>
        /// 匹配检查
        /// 确定此定位器是否能处理指定的数据对象
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns>是否能处理该数据对象</returns>
        public bool Match(object? data)
        {
            return data is ViewModelBase;  // 只处理ViewModelBase派生类
        }
    }
}
