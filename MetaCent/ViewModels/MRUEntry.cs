using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;

namespace Meowtrix.MetaCent.ViewModels
{
    public sealed class MRUEntry : INotifyPropertyChanged
    {
        public string Title { get; }
        public string Token { get; }
        public string Path { get; private set; }
        private readonly Task<StorageFolder> folderTask;

        private readonly MRUList list;

        public event PropertyChangedEventHandler PropertyChanged;

        public MRUEntry(MRUList list, string title, string token, IAsyncOperation<StorageFolder> folderTask)
        {
            this.list = list;
            Title = title;
            Token = token;
            this.folderTask = folderTask.AsTask();
            UpdateFolder();
        }

        public MRUEntry(MRUList list, string title, string token, StorageFolder folder)
        {
            this.list = list;
            Title = title;
            Token = token;
            folderTask = Task.FromResult(folder);
        }

        public async void UpdateFolder()
        {
            var folder = await folderTask;
            if (!string.IsNullOrEmpty(folder?.Path))
            {
                Path = folder.Path;
                await list.Dispatcher.RunAsync(CoreDispatcherPriority.High, () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path))));
            }
        }

        public Task<StorageFolder> GetFolderAsync() => folderTask;

        public void Remove() => list.Remove(this);
    }
}
