namespace Pileolinks.Components.Tree;

public partial class Tree : ContentView
{
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(List<ITreeItem>), typeof(Tree), new List<ITreeItem>(), propertyChanged: OnItemsChanged);

    public TreeViewModel ViewModel { get; set; }

    public List<ITreeItem> Items
    {
        get => (List<ITreeItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public Tree()
    {
        InitializeComponent();
    }

    public static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Tree tree = bindable as Tree;
        TreeViewModel newViewModel = new((List<ITreeItem>)newValue);
        tree.ViewModel = newViewModel;
        tree.BindingContext = tree.ViewModel;
        
    }

}
