using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.ViewModels;

namespace Pileolinks;

public partial class DirectoryView : ContentPage
{
	public DirectoryViewModel ViewModel { get; private set; }
	public DirectoryView(ITreeItem item = null)
	{
		InitializeComponent();
		BindingContext = ViewModel = new DirectoryViewModel((LinkDirectory)item);
        ViewModel.DirectorySelected += ViewModel_DirectorySelected;
        ViewModel.LinkAdded += ViewModel_LinkAdded;
	}

    private async void ViewModel_LinkAdded(object sender, LinkViewModel e)
    {
        await Navigation.PushAsync(new LinkView(e.Link));
    }

    private async void ViewModel_DirectorySelected(object sender, DirectoryViewModel e)
    {
        await Navigation.PushAsync(new DirectoryView(e.LinkDirectory));
    }

    public DirectoryView()
    {
		InitializeComponent();
		BindingContext = ViewModel = new DirectoryViewModel(null);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.Initialise();
    }
}