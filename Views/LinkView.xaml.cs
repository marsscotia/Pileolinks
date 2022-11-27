using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.ViewModels;

namespace Pileolinks.Views;

public partial class LinkView : ContentPage
{
	public LinkViewModel ViewModel { get; private set; }
	public LinkView(ITreeItem item = null)
	{
		InitializeComponent();
        BindingContext = ViewModel = new LinkViewModel((Link)item);
	}

	public LinkView()
    {
		InitializeComponent();
		BindingContext = ViewModel = new LinkViewModel(null);
    }
}