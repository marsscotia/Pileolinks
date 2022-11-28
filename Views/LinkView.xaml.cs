using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.ViewModels;
using Pileolinks.ViewModels.Factories;
using Pileolinks.ViewModels.Factories.Interfaces;

namespace Pileolinks.Views;

public partial class LinkView : ContentPage
{
	public LinkViewModel ViewModel { get; private set; }
	public LinkView(ITreeItem item = null)
	{
		InitializeComponent();
		ILinkViewModelFactory linkViewModelFactory = App.Current.ServiceProvider.GetService<ILinkViewModelFactory>();
        BindingContext = ViewModel = linkViewModelFactory.GetLinkViewModel((Link)item);
	}

	public LinkView()
    {
		InitializeComponent();
        ILinkViewModelFactory linkViewModelFactory = App.Current.ServiceProvider.GetService<ILinkViewModelFactory>();
        BindingContext = ViewModel = linkViewModelFactory.GetLinkViewModel(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.Leaving();
    }
}