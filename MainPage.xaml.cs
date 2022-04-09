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
        await TreeNavigation.PopAsync();
        await TreeNavigation.PushAsync(new DirectoryView(TreeFlyout.ViewModel.InitialNavigate));
    }

    private async void Flyout_TreeItemViewModelChanged(object sender, Components.Tree.ITreeItem e)
    {
        await TreeNavigation.Navigation.PushAsync(new DirectoryView(e));
    }
}

