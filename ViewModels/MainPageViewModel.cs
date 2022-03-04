using Pileolinks.Components.Tree;
using Pileolinks.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private List<ITreeItem> items = new();
        
        public List<ITreeItem> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public MainPageViewModel()
        {
            Items = new MockDataService().GetTopLevelTreeItems();
        }

    }
}
