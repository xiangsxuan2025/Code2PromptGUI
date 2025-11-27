using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code2LlmPrompt.ViewModels
{
    public partial class MainViewModel
    {
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
    }
}
