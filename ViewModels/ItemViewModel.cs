using Pileolinks.Components.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public class ItemViewModel: BaseViewModel
    {
        protected readonly ITreeItem item;

        public ITreeItem Item => item;

        public TreeItemType Type => item.Type;

        public bool NoItem => item == null;

        public ItemViewModel(ITreeItem item)
        {
            this.item = item;
        }


    }
}
