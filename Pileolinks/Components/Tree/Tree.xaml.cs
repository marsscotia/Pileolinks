using System.Collections.ObjectModel;
using System.Timers;

namespace Pileolinks.Components.Tree;

public partial class Tree : ContentView
{
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(ObservableCollection<ITreeItem>), typeof(Tree), new ObservableCollection<ITreeItem>(), defaultBindingMode: BindingMode.OneWay, propertyChanged: OnItemsChanged);

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(ITreeItem), typeof(Tree), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

    private System.Timers.Timer ExpandTimer;

    public event EventHandler<string> InvalidMoveRequested;
    public event EventHandler<ITreeItem> SelectedItemChanged;

    public ObservableCollection<ITreeItemViewModel> ItemsSource { get; private set; } = new();

    public ObservableCollection<ITreeItem> Items
    {
        get => (ObservableCollection<ITreeItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public ITreeItem SelectedItem
    {
        get => (ITreeItem)GetValue(SelectedItemProperty);
        set
        {
            SetValue(SelectedItemProperty, value);
            SelectedItemChanged?.Invoke(this, ItemsSource.First(i => i.Id == value.Id).TreeItem);
        }
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
        treeItemViewModel.Deleted += ItemDeleted;
        treeItemViewModel.ItemMoved += MoveItem;
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
        foreach (ITreeItemViewModel item in ItemsSource.Where(i => i.IsSelected))
        {
            item.IsSelected = false;
        }
        ITreeItemViewModel viewModel = ItemsSource.FirstOrDefault(i => i.TreeItem.Id == selected?.Id);
        if (viewModel != null)
        {
            viewModel.IsSelected = true;
        }
    }


    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        SelectedItem = ((ITreeItemViewModel)((Label)sender).BindingContext).TreeItem;
    }

    private void ItemDeleted(object sender, EventArgs e)
    {
        ITreeItemViewModel selected = sender as ITreeItemViewModel;
        Stack<ITreeItemViewModel> stack = new();
        stack.Push(selected);

        List<string> ids = new();

        while (stack.Any())
        {
            ITreeItemViewModel current = stack.Pop();

            ids.Add(current.Id);

            foreach (ITreeItemViewModel item in current.Descendants)
            {
                stack.Push(item);
            }
        }

        foreach (string id in ids)
        {
            ITreeItemViewModel treeItemViewModel = ItemsSource.FirstOrDefault(i => i.Id == id);
            if (treeItemViewModel is not null)
            {
                if (SelectedItem == treeItemViewModel.TreeItem)
                {
                    SelectedItem = null;
                }
                treeItemViewModel.DescendantAdded -= AddDescendant;
                treeItemViewModel.Deleted -= ItemDeleted;
                treeItemViewModel.ItemMoved -= MoveItem;
                _ = ItemsSource.Remove(treeItemViewModel);
            }
        }

        if (selected.HasAncestor)
        {
            ITreeItemViewModel ancestor = ItemsSource.FirstOrDefault(i => i.Id == selected.Ancestor.Id);
            if (ancestor is not null)
            {
                ancestor.RemoveDescendant(selected);
            }
        }


    }

    private int AddDescendant(ITreeItemViewModel ancestor, ITreeItemViewModel treeItemViewModel)
    {
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
                IEnumerable<ITreeItemViewModel> laterAncestors = ItemsSource.Where(i => i.Depth <= ancestor.Depth && ItemsSource.IndexOf(i) > ancestorIndex);
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
        return ItemsSource.IndexOf(treeItemViewModel);
    }

    private void AddDescendant(object sender, ITreeItemViewModel treeItemViewModel)
    {
        treeItemViewModel.DescendantAdded += AddDescendant;
        treeItemViewModel.Deleted += ItemDeleted;
        treeItemViewModel.ItemMoved += MoveItem;

        ITreeItemViewModel ancestor = sender as ITreeItemViewModel;
        
        _ = AddDescendant(ancestor, treeItemViewModel);
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
            treeItemViewModel.Deleted += ItemDeleted;
            treeItemViewModel.ItemMoved += MoveItem;
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
                ancestor.AddToDescendants(item);
            }
            else
            {
                item.IsVisible = true;
            }
        }

        OnPropertyChanged(nameof(ItemsSource));
    }

    private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
    {
        Label label = (sender as Element).Parent as Label;
        ITreeItemViewModel selected = label.BindingContext as ITreeItemViewModel;
        e.Data.Properties.Add("ITreeItemViewModel", selected);
    }

    private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
    {
        Label label = (sender as Element).Parent as Label;
        ITreeItemViewModel selected = label.BindingContext as ITreeItemViewModel;
        ITreeItemViewModel dropped = e.Data.Properties["ITreeItemViewModel"] as ITreeItemViewModel;
        selected.IsHovered = false;
        bool valid = dropped.TreeItem.MoveToDirectory(selected.TreeItem);
        if (!valid)
        {
            InvalidMoveRequested.Invoke(this, "That directory can't be moved there, there's a directory with the same name already there.");
        }
        e.Handled = true;
    }

    private void DropGestureRecognizer_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
        Label label = (sender as Element).Parent as Label;
        ITreeItemViewModel selected = label.BindingContext as ITreeItemViewModel;
        selected.IsHovered = true;
        ExpandTimer = new(1000);
        ExpandTimer.Elapsed += (s, e) =>
        {
            if (selected.CanExpandAndIsNotExpanded)
            {
                selected.IsExpanded = true;
                ExpandTimer.Stop();
                ExpandTimer.Dispose();
            }
        };
        ExpandTimer.AutoReset = false;
        ExpandTimer.Start();
    }

    private void DropGestureRecognizer_DragLeave(object sender, DragEventArgs e)
    {
        Label label = (sender as Element).Parent as Label;
        ITreeItemViewModel selected = label.BindingContext as ITreeItemViewModel;
        selected.IsHovered = false;
        if (ExpandTimer != null && ExpandTimer.Enabled)
        {
            ExpandTimer.Stop();
            ExpandTimer.Dispose();
        }
    }

    private void MoveItem(object sender, ITreeItem newAncestor)
    {
        ITreeItemViewModel moved = sender as ITreeItemViewModel;
        
        ITreeItemViewModel newAncestorViewModel = ItemsSource.First(t => t.Id == newAncestor.Id);
        ITreeItemViewModel oldAncestor = ItemsSource.FirstOrDefault(t => t.Descendants.Any(d => d.Id == moved.Id));
        if (oldAncestor is not null)
        {
            oldAncestor.RemoveDescendant(moved); 
        }
        _ = ItemsSource.Remove(moved);
        newAncestorViewModel.AddToDescendants(moved);
        int newIndex = AddDescendant(newAncestorViewModel, moved);

        List<ITreeItemViewModel> moving = new();
        Stack<ITreeItemViewModel> stack = new();

        foreach (ITreeItemViewModel treeItemViewModel in moved.Descendants.OrderByDescending(i => i.Name))
        {
            stack.Push(treeItemViewModel);
        }

        while (stack.Any())
        {
            ITreeItemViewModel current = stack.Pop();
            moving.Add(current);

            foreach (ITreeItemViewModel treeItemViewModel in current.Descendants.OrderByDescending(i => i.Name))
            {
                stack.Push(treeItemViewModel);
            }
        }

        foreach (ITreeItemViewModel item in moving)
        {
            ItemsSource.Move(ItemsSource.IndexOf(item), newIndex + 1);
            newIndex++;
        }

    }
}


