using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services;
using System.Collections.ObjectModel;

namespace Pileolinks.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<ITreeItem> items = new();
        private ITreeItem selected;
        private string categoryName, collectionName;
        private Command addCategoryCommand, addCollectionCommand;

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

        public string CategoryName
        {
            get => categoryName;
            set => SetProperty(ref categoryName, value);
        }

        public string CollectionName
        {
            get => collectionName;
            set => SetProperty(ref collectionName, value);
        }

        public string SelectedSummary => Selected == null ?
                    "There is no currently selected directory" :
                    $"{Selected.Name} is the currently selected directory";

        public Command AddDirectoryCommand => addCategoryCommand ??= new Command(AddDirectory);
        public Command AddCollectionCommand => addCollectionCommand ??= new Command(AddCollection);

        public MainPageViewModel()
        {
            ObservableCollection<ITreeItem> items = new();
            foreach (ITreeItem item in new MockDataService().GetTopLevelTreeItems())
            {
                items.Add(item);
            }
            Items = items;
        }

        private void AddDirectory()
        {
            if (Selected != null && !string.IsNullOrWhiteSpace(CategoryName))
            {
                Selected.AddDirectory(CategoryName); 
            }
            CategoryName = string.Empty;
        }

        private void AddCollection()
        {
            if (!string.IsNullOrWhiteSpace(CollectionName))
            {
                if (!Items.Any(i => i.Name.Equals(CollectionName, StringComparison.OrdinalIgnoreCase)))
                {
                    LinkDirectory linkDirectory = new(Guid.NewGuid().ToString(), null, CollectionName, new List<ITreeItem>());
                    int count = 0;
                    ITreeItem found = null;
                    while(found == null && count < Items.Count)
                    {
                        if (string.Compare(CollectionName, Items.ElementAt(count).Name, StringComparison.OrdinalIgnoreCase) <= 0)
                        {
                            found = Items.ElementAt(count);
                        }
                        count++;
                    }
                    if (found == null)
                    {
                        Items.Add(linkDirectory);
                    }
                    else
                    {
                        Items.Insert(Items.IndexOf(found), linkDirectory);
                    }
                    CollectionName = String.Empty;
                }
            }
        }

    }
}
