using Pileolinks.Components.Tree;
using Pileolinks.Models;

namespace Pileolinks.Services.Interfaces
{
    public interface IDataService
    {
        List<ITreeItem> GetTopLevelTreeItems();
        bool SaveCollection(LinkDirectory linkDirectory);
        void SaveCollections(List<LinkDirectory> linkDirectories);
    }
}
