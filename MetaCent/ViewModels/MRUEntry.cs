using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Meowtrix.MetaCent.ViewModels
{
    public sealed class MRUEntry : INotifyPropertyChanged
    {
        public string Title { get; }
        public string Token { get; }
        public string Path { get; private set; }
        private readonly Task<StorageFolder> folderTask;

        public event PropertyChangedEventHandler PropertyChanged;

        public MRUEntry(string title, string token, IAsyncOperation<StorageFolder> folderTask)
        {
            Title = title;
            Token = token;
            this.folderTask = folderTask.AsTask();
            UpdateFolder();
        }

        public MRUEntry(string title, string token, StorageFolder folder)
        {
            Title = title;
            Token = token;
            Path = folder.Path;
            folderTask = Task.FromResult(folder);
        }

        public async void UpdateFolder()
        {
            var folder = await folderTask;
            if (!string.IsNullOrEmpty(folder?.Path))
            {
                Path = folder.Path;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
            }
        }

        public Task<StorageFolder> GetFolderAsync() => folderTask;
    }
}
