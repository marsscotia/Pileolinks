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
        private readonly ITreeItem item;

        public ITreeItem Item => item;

        public TreeItemType Type => item.Type;

        public bool NoItem => item == null;

        public string Name => item == null ? string.Empty : item.Name;
        
        public ItemViewModel(ITreeItem item)
        {
            this.item = item;
        }


    }
}
