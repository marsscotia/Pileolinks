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
        tree.SetItems((ObservableCollection<ITreeItem>)newValue);
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
        SelectedItem = ((ITreeItemViewModel)(((Label)sender).BindingContext)).TreeItem;
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
            ItemsSource.Add(new TreeItemViewModel(current));

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


