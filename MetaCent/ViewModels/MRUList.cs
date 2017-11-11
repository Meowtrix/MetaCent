using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Meowtrix.MetaCent.ViewModels
{
    public class MRUList
    {
        private readonly StorageItemMostRecentlyUsedList mru;
        private readonly ObservableCollection<MRUEntry> innerList;

        public ReadOnlyObservableCollection<MRUEntry> Entries { get; }

        public MRUList()
        {
            mru = StorageApplicationPermissions.MostRecentlyUsedList;
            innerList = new ObservableCollection<MRUEntry>(mru.Entries.Select(x => new MRUEntry(x.Metadata, x.Token, mru.GetFolderAsync(x.Token))));
            Entries = new ReadOnlyObservableCollection<MRUEntry>(innerList);
        }

        public void Add(StorageFolder folder, string title)
        {
            string token = mru.Add(folder, title);
            var entry = new MRUEntry(title, token, folder);
            innerList.Insert(0, entry);
        }

        public void Remove(MRUEntry entry)
        {
            mru.Remove(entry.Token);
            innerList.Remove(entry);
        }
    }
}
