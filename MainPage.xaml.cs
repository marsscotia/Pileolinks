using Pileolinks.ViewModels;

namespace Pileolinks;

public partial class MainPage : ContentPage
{
	public MainPageViewModel ViewModel { get; private set; }
	public MainPage()
	{
		InitializeComponent();
		BindingContext = ViewModel = new MainPageViewModel();
	}
}

