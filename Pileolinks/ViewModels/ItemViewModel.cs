using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;

namespace Pileolinks.ViewModels
{
    public partial class ItemViewModel: BaseViewModel
    {
        protected readonly ITreeItem item;
        protected readonly IDataService dataService;

        public ITreeItem Item => item;

        public TreeItemType Type => item.Type;

        public bool NoItem => item == null;

        public ItemViewModel(IDataService dataService, ITreeItem item)
        {
            this.item = item;
            this.dataService = dataService;
        }

        protected LinkDirectory GetTopLevelAncestor()
        {
            if (Type == TreeItemType.Directory && !item.HasAncestor)
            {
                return (LinkDirectory)item;
            }
            else
            {
                LinkDirectory ancestor = (LinkDirectory)item.Ancestor;
                while (ancestor.HasAncestor)
                {
                    ancestor = (LinkDirectory)(ancestor.Ancestor);
                }
                return ancestor;
            }
        }
    }
}
