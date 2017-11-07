using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Core;

namespace Meowtrix.MetaCent.ViewModels
{
    public sealed class MRUEntry : INotifyPropertyChanged
    {
        public string Title { get; }
        public string Token { get; }
        public string Path { get; private set; }
        public StorageFolder Folder { get; private set; }

        private readonly MRUList list;

        public event PropertyChangedEventHandler PropertyChanged;

        public MRUEntry(MRUList list, string title, string token)
        {
            this.list = list;
            Title = title;
            Token = token;
        }

        public async void SetStorage(StorageFolder folder)
        {
            Folder = folder;
            if (!string.IsNullOrEmpty(folder.Path))
            {
                Path = folder.Path;
                await list.Dispatcher.RunAsync(CoreDispatcherPriority.High, () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path))));
            }
        }

        public void Remove() => list.Remove(this);
    }
}
