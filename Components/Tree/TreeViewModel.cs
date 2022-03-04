using System.Collections.ObjectModel;

namespace Pileolinks.Components.Tree
{
    public class TreeViewModel : BaseViewModel, ITreeViewModel
    {
        public ObservableCollection<ITreeItemViewModel> Items { get; private set; } = new();

        public TreeViewModel(ObservableCollection<ITreeItemViewModel> items)
        {
            Items = items;
        }

        public TreeViewModel(List<ITreeItem> items)
        {
            PopulateItems(items);
            OnPropertyChanged(nameof(Items));
        }

        private void PopulateItems(List<ITreeItem> items)
        {
            Stack<ITreeItem> stack = new();

            foreach (ITreeItem item in items.OrderByDescending(i => i.Name))
            {
                stack.Push(item);
            }

            while (stack.Count > 0)
            {
                ITreeItem current = stack.Pop();
                Items.Add(new TreeItemViewModel(current));

                foreach (ITreeItem item in current.Directories.OrderByDescending(i => i.Name))
                {
                    stack.Push(item);
                }
            }

            foreach (ITreeItemViewModel item in Items)
            {
                if (item.HasAncestor)
                {
                    ITreeItemViewModel ancestor = Items.FirstOrDefault(i => i.Id == item.Ancestor.Id);
                    InsertIntoDescendants(item, ancestor.Descendants);
                }
                else
                {
                    item.IsVisible = true;
                }
            }
        }

        private static void InsertIntoDescendants(ITreeItemViewModel descendant, List<ITreeItemViewModel> descendants)
        {
            if (!descendants.Any())
            {
                descendants.Add(descendant);
            }
            else
            {
                bool found = false;
                int counter = 0;
                while (!found && counter < descendants.Count)
                {
                    ITreeItemViewModel current = descendants[counter];
                    if(string.Compare(descendant.Name, current.Name) <= 0)
                    {
                        found = true;
                        descendants.Insert(counter, descendant);
                    }
                    counter++;
                }
                if (!found)
                {
                    descendants.Add(descendant);
                }
            }
        }

    }
}
