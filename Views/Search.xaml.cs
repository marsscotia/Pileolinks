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
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		ViewModel.Arriving();
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