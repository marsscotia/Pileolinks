using System.Collections.ObjectModel;

namespace Pileolinks.Components.Tree;

public partial class Tree : ContentView
{
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(ObservableCollection<ITreeItem>), typeof(Tree), new ObservableCollection<ITreeItem>(), defaultBindingMode: BindingMode.OneWay, propertyChanged: OnItemsChanged);

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(ITreeItem), typeof(Tree), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

    public ObservableCollection<ITreeItemViewModel> ItemsSource { get; private set; } = new();

    public ObservableCollection<ITreeItem> Items
    {
        get => (ObservableCollection<ITreeItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public ITreeItem SelectedItem
    {
        get => (ITreeItem)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public Tree()
    {
        InitializeComponent();
    }

    public static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Tree tree = bindable as Tree;
        ObservableCollection<ITreeItem> oldItems = oldValue as ObservableCollection<ITreeItem>;
        ObservableCollection<ITreeItem> newItems = newValue as ObservableCollection<ITreeItem>;
        tree.ItemsChanged(oldItems, newItems);
    }

    private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            AddTopLevelDirectory(e.NewItems[0] as ITreeItem); 
        }
    }

    private void AddTopLevelDirectory(ITreeItem treeItem)
    {
        TreeItemViewModel treeItemViewModel = new(treeItem);
        treeItemViewModel.DescendantAdded += AddDescendant;
        treeItemViewModel.IsVisible = true;

        int count = 0;
        ITreeItemViewModel found = null;
        IEnumerable<ITreeItemViewModel> collections = ItemsSource.Where(i => !i.HasAncestor);
        while (found == null && count < collections.Count())
        {
            if (string.Compare(treeItemViewModel.Name, collections.ElementAt(count).Name, StringComparison.OrdinalIgnoreCase) <= 0)
            {
                found = collections.ElementAt(count);
            }
            count++;
        }
        if (found == null)
        {
            ItemsSource.Add(treeItemViewModel);
        }
        else
        {
            ItemsSource.Insert(ItemsSource.IndexOf(found), treeItemViewModel);
        }
    }

    private void ItemsChanged(ObservableCollection<ITreeItem> oldItems, ObservableCollection<ITreeItem> newItems)
    {
        if (oldItems != null)
        {
            oldItems.CollectionChanged -= Items_CollectionChanged;
        }
        if (newItems != null)
        {
            newItems.CollectionChanged += Items_CollectionChanged;
            SetItems(newItems);
        }
    }

    public static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Tree tree = bindable as Tree;
        tree.SetSelectedItem(newValue as ITreeItem);
    }

    private void SetItems(ObservableCollection<ITreeItem> items)
    {
        PopulateItems(items);
    }

    private void SetSelectedItem(ITreeItem selected)
    {
        ITreeItemViewModel viewModel = ItemsSource.FirstOrDefault(i => i.TreeItem == selected);
        if (viewModel != null)
        {
            foreach (ITreeItemViewModel item in ItemsSource.Where(i => i.IsSelected))
            {
                item.IsSelected = false;
            }
            viewModel.IsSelected = true;
        }
    }


    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        SelectedItem = ((ITreeItemViewModel)((Label)sender).BindingContext).TreeItem;
    }

    private void AddDescendant(object sender, ITreeItemViewModel treeItemViewModel)
    {
        treeItemViewModel.DescendantAdded += AddDescendant;

        ITreeItemViewModel ancestor = sender as ITreeItemViewModel;
        int ancestorIndex = ItemsSource.IndexOf(ancestor);
        IEnumerable<ITreeItemViewModel> siblings = ItemsSource.Where(i => i.Depth == treeItemViewModel.Depth && i.Ancestor == treeItemViewModel.Ancestor);
        if (siblings.Any())
        {
            int count = 0;
            ITreeItemViewModel found = null;
            while (found == null && count < siblings.Count())
            {
                if (string.Compare(treeItemViewModel.Name, siblings.ElementAt(count).Name, StringComparison.OrdinalIgnoreCase) <= 0)
                {
                    found = siblings.ElementAt(count);
                }
                count++;
            }
            if (found == null)
            {
                IEnumerable<ITreeItemViewModel> laterAncestors = ItemsSource.Where(i => i.Depth == ancestor.Depth && ItemsSource.IndexOf(i) > ancestorIndex);
                if (laterAncestors.Any())
                {
                    int minLaterAncestorIndex = laterAncestors.Select(i => ItemsSource.IndexOf(i)).Min();
                    ItemsSource.Insert(minLaterAncestorIndex, treeItemViewModel);
                }
                else
                {
                    ItemsSource.Add(treeItemViewModel);
                }
            }
            else
            {
                ItemsSource.Insert(ItemsSource.IndexOf(found), treeItemViewModel);
            }
        }
        else
        {
            IEnumerable<ITreeItemViewModel> laterAncestors = ItemsSource.Where(i => i.Depth == ancestor.Depth && ItemsSource.IndexOf(i) > ancestorIndex);
            if (laterAncestors.Any())
            {
                int minLaterAncestorsIndex = laterAncestors.Select(i => ItemsSource.IndexOf(i)).Min();
                ItemsSource.Insert(minLaterAncestorsIndex, treeItemViewModel);
            }
            else
            {
                ItemsSource.Add(treeItemViewModel);
            }
        }
        treeItemViewModel.IsVisible = ancestor.IsVisible && ancestor.CanExpandAndIsExpanded;

    }

    private void PopulateItems(ObservableCollection<ITreeItem> items)
    {
        ItemsSource.Clear();
        Stack<ITreeItem> stack = new();

        foreach (ITreeItem item in items.OrderByDescending(i => i.Name))
        {
            stack.Push(item);
        }

        while (stack.Count > 0)
        {
            ITreeItem current = stack.Pop();
            TreeItemViewModel treeItemViewModel = new(current);
            treeItemViewModel.DescendantAdded += AddDescendant;
            ItemsSource.Add(treeItemViewModel);

            foreach (ITreeItem item in current.Directories.OrderByDescending(i => i.Name))
            {
                stack.Push(item);
            }
        }

        foreach (ITreeItemViewModel item in ItemsSource)
        {
            if (item.HasAncestor)
            {
                ITreeItemViewModel ancestor = ItemsSource.FirstOrDefault(i => i.Id == item.Ancestor.Id);
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
                if (string.Compare(descendant.Name, current.Name) <= 0)
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


