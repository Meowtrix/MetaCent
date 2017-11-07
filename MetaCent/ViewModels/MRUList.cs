using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Core;

namespace Meowtrix.MetaCent.ViewModels
{
    public class MRUList
    {
        private readonly StorageItemMostRecentlyUsedList mru;
        private readonly ObservableCollection<MRUEntry> innerList;

        internal readonly CoreDispatcher Dispatcher;
        public ReadOnlyObservableCollection<MRUEntry> Entries { get; }

        public MRUList(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            mru = StorageApplicationPermissions.MostRecentlyUsedList;
            innerList = new ObservableCollection<MRUEntry>(mru.Entries.Select(x => new MRUEntry(this, x.Metadata, x.Token)));
            Entries = new ReadOnlyObservableCollection<MRUEntry>(innerList);
            FetchPaths();
        }

        private async void FetchPaths()
        {
            int count = innerList.Count;
            for (int i = 0; i < count; i++)
            {
                var entry = innerList[i];
                var folder = await mru.GetFolderAsync(entry.Token);
                if (folder != null)
                    entry.SetStorage(folder);
            }
        }

        public async void Add(StorageFolder folder, string title)
        {
            string token = mru.Add(folder, title);
            var entry = new MRUEntry(this, title, token);
            entry.SetStorage(folder);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => innerList.Insert(0, entry));
        }

        public async void Remove(MRUEntry entry)
        {
            mru.Remove(entry.Token);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => innerList.Remove(entry));
        }
    }
}
