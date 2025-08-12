using Pileolinks.ViewModels;
using Pileolinks.Views;

namespace Pileolinks;

public partial class MainPage : FlyoutPage
{
	public TreeFlyout TreeFlyout { get; private set; }
    
    public MainPage(TreeFlyout treeFlyout)
	{
		InitializeComponent();
		BindingContext = this;
        Flyout = TreeFlyout = treeFlyout;
        treeFlyout.TreeItemViewModelChanged += Flyout_TreeItemViewModelChanged;
        treeFlyout.Loaded += TreeFlyout_Loaded;
        TreeNavigation.Pushed += TreeNavigation_Pushed;
        TreeNavigation.Popped += TreeNavigation_Popped;
    }

    private void TreeNavigation_Popped(object sender, NavigationEventArgs e)
    {
        e.Page.NavigatedFrom -= TreeNavigation_NavigatedFrom;
        e.Page.NavigatedTo -= TreeNavigation_NavigatedTo;
    }

    private void TreeNavigation_Pushed(object sender, NavigationEventArgs e)
    {
        SetCurrentSelectedItem(e.Page);
        e.Page.NavigatedTo += TreeNavigation_NavigatedTo;
        e.Page.NavigatedFrom += TreeNavigation_NavigatedFrom;
    }

    /// <summary>
    /// Notify the tree when we have navigated to a new directory view so it can 
    /// display the correct selected directory
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeNavigation_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        SetCurrentSelectedItem(TreeNavigation.CurrentPage);
    }

    private void SetCurrentSelectedItem(Page page)
    {
        TreeFlyout.ViewModel.Selected = null;
        var currentViewModel = page.BindingContext;
        if (currentViewModel is not null and DirectoryViewModel)
        {
            TreeFlyout.ViewModel.Selected = ((DirectoryViewModel)currentViewModel).Item;
        }
    }

    private void TreeNavigation_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        TreeFlyout.ViewModel.Selected = null;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        DirectoryView initial = new(TreeFlyout.ViewModel.InitialNavigate);
        initial.NavigatedTo += TreeNavigation_NavigatedTo;
        initial.NavigatedFrom += TreeNavigation_NavigatedFrom;
        await TreeNavigation.PushAsync(initial);
        SetCurrentSelectedItem(TreeNavigation.CurrentPage);
    }

    private async void Flyout_TreeItemViewModelChanged(object sender, Components.Tree.ITreeItem e)
    {
        await TreeNavigation.Navigation.PushAsync(new DirectoryView(e));
    }

    public void SaveState()
    {
        TreeFlyout.ViewModel.SaveState();
    }

    private async void TreeFlyout_Loaded(object sender, EventArgs e)
    {
        if (!TreeFlyout.ViewModel.Items.Any())
        {
            string collectionName = string.Empty;
            while (string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = await DisplayPromptAsync("First Collection", 
                    "Welcome! Pile o' Links stores links and folders in Collections. What would you like to call your first collection?");
            }
            TreeFlyout.ViewModel.AddCollectionCommand.Execute(collectionName);
            await TreeNavigation.PopAsync();
            await TreeNavigation.PushAsync(new DirectoryView(TreeFlyout.ViewModel.InitialNavigate));
        }
    }
}

