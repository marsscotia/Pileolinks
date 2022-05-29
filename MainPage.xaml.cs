using Pileolinks.ViewModels;

namespace Pileolinks;

public partial class MainPage : FlyoutPage
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await TreeNavigation.PushAsync(new DirectoryView(TreeFlyout.ViewModel.InitialNavigate));
        
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

