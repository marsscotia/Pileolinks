using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.ViewModels;

namespace Pileolinks.Views;

public partial class DirectoryView : ContentPage
{
	public DirectoryViewModel ViewModel { get; private set; }
	public DirectoryView(ITreeItem item = null)
	{
		InitializeComponent();
		BindingContext = ViewModel = new DirectoryViewModel((LinkDirectory)item);
        ViewModel.DirectorySelected += ViewModel_DirectorySelected;
        ViewModel.EditLinkRequested += ViewModel_EditLinkRequested;
        ViewModel.LaunchUrlRequested += ViewModel_LaunchUrlRequested;
        ViewModel.AlertRequested += ViewModel_AlertRequested;
        ViewModel.SearchRequested += ViewModel_SearchRequested;
	}

    private async void ViewModel_SearchRequested(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Search());
    }

    private async void ViewModel_LaunchUrlRequested(object sender, string e)
    {
        try
        {
            Uri uri = new(e);
            await Browser.Default.OpenAsync(uri);
        }
        catch (Exception)
        {
            await DisplayAlert("Something went wrong", "We couldn't open this link in your browser", "OK");
        }
    }

    private async void ViewModel_AlertRequested(object sender, AlertEventArgs e)
    {
        await DisplayAlert(e.Title, e.Message, e.ConfirmButton);
    }

    private async void ViewModel_EditLinkRequested(object sender, LinkViewModel e)
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

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Directory", "What would you like to call the new directory?");
        if (name is not null)
        {
            ViewModel.AddDirectoryCommand.Execute(name);
        }
    }
}