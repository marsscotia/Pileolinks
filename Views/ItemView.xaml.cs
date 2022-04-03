using Pileolinks.Components.Tree;
using Pileolinks.ViewModels;

namespace Pileolinks;

public partial class ItemView : ContentPage
{
	public ItemViewModel ViewModel { get; private set; }
	public ItemView(ITreeItem item = null)
	{
		InitializeComponent();
        BindingContext = ViewModel = new ItemViewModel(item);
	}

	public ItemView()
    {
		InitializeComponent();
		BindingContext = ViewModel = new ItemViewModel(null);
    }
}