using Pileolinks.Components.Tree;
using Pileolinks.Services;
using System.Collections.ObjectModel;

namespace Pileolinks.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<ITreeItem> items = new();
        private ITreeItem selected;

        public ObservableCollection<ITreeItem> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public ITreeItem Selected
        {
            get => selected;
            set
            {
                SetProperty(ref selected, value);
                OnPropertyChanged(nameof(SelectedSummary));
            }
        }

        public string SelectedSummary => Selected == null ?
                    "There is no currently selected directory" :
                    $"{Selected.Name} is the currently selected directory";

        public MainPageViewModel()
        {
            ObservableCollection<ITreeItem> items = new();
            foreach (ITreeItem item in new MockDataService().GetTopLevelTreeItems())
            {
                items.Add(item);
            }
            Items = items;
        }

    }
}
