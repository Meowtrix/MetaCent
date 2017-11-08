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
            innerList = new ObservableCollection<MRUEntry>(mru.Entries.Select(x => new MRUEntry(Dispatcher, x.Metadata, x.Token, mru.GetFolderAsync(x.Token))));
            Entries = new ReadOnlyObservableCollection<MRUEntry>(innerList);
        }

        public async void Add(StorageFolder folder, string title)
        {
            string token = mru.Add(folder, title);
            var entry = new MRUEntry(Dispatcher, title, token, folder);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => innerList.Insert(0, entry));
        }

        public async void Remove(MRUEntry entry)
        {
            mru.Remove(entry.Token);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => innerList.Remove(entry));
        }
    }
}
