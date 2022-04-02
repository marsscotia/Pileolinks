using Pileolinks.Components.Tree;

namespace Pileolinks.Services.Interfaces
{
    internal interface IDataService
    {
        List<ITreeItem> GetTopLevelTreeItems();
    }
}
