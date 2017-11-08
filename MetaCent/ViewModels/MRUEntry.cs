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

        private readonly CoreDispatcher dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public MRUEntry(CoreDispatcher dispatcher, string title, string token, IAsyncOperation<StorageFolder> folderTask)
        {
            this.dispatcher = dispatcher;
            Title = title;
            Token = token;
            this.folderTask = folderTask.AsTask();
            UpdateFolder();
        }

        public MRUEntry(CoreDispatcher dispatcher, string title, string token, StorageFolder folder)
        {
            this.dispatcher = dispatcher;
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
                await dispatcher.RunAsync(CoreDispatcherPriority.High, () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path))));
            }
        }

        public Task<StorageFolder> GetFolderAsync() => folderTask;
    }
}
