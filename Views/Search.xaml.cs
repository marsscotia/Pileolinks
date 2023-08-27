using Pileolinks.ViewModels;

namespace Pileolinks.Views;

public partial class Search : ContentPage
{
	public SearchViewModel ViewModel { get; private set; }

	public Search(SearchViewModel searchViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel = searchViewModel;
		ViewModel.AlertRequested += AlertRequested;
        ViewModel.EditLinkRequested += ViewModel_EditLinkRequested;
        ViewModel.LaunchUrlRequested += ViewModel_LaunchUrlRequested;
        ViewModel.OpenParentRequested += ViewModel_OpenParentRequested;
	}

    private async void ViewModel_OpenParentRequested(object sender, Models.LinkDirectory e)
    {
        await Navigation.PushAsync(new DirectoryView(e));
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

    private async void ViewModel_EditLinkRequested(object sender, LinkViewModel e)
    {
		await Navigation.PushAsync(new LinkView(e.Link));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
		ViewModel.Arriving();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		ViewModel.AlertRequested -= AlertRequested;
		ViewModel.EditLinkRequested -= ViewModel_EditLinkRequested;
		ViewModel.LaunchUrlRequested -= ViewModel_LaunchUrlRequested;
		ViewModel.OpenParentRequested -= ViewModel_OpenParentRequested;
		ViewModel.Leaving();
    }

    private async void AlertRequested(object sender, DialogRequestedEventArgs dialogRequestedEventArgs)
	{
		if (dialogRequestedEventArgs.Cancel == null)
		{
			await DisplayAlert(dialogRequestedEventArgs.Title, dialogRequestedEventArgs.Message, dialogRequestedEventArgs.Confirm);
		}
		else 
		{
            await DisplayAlert(dialogRequestedEventArgs.Title, dialogRequestedEventArgs.Message, dialogRequestedEventArgs.Confirm, dialogRequestedEventArgs.Cancel);
        }
		
		
	}
}