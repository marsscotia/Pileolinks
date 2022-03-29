using Pileolinks.ViewModels;

namespace Pileolinks;

public partial class MainPage : ContentPage
{
	public MainPageViewModel ViewModel { get; private set; }
	public MainPage()
	{
		InitializeComponent();
		BindingContext = ViewModel = new MainPageViewModel();
        ViewModel.DeleteRequested += ViewModel_DeleteRequested;
        ViewModel.AlertRequested += ViewModel_AlertRequested;
        ViewModel.AddCollectionRequested += ViewModel_AddCollectionRequested;
        ViewModel.AddDirectoryRequested += ViewModel_AddDirectoryRequested;
    }

    private async void ViewModel_AddDirectoryRequested(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Directory", "What would you like to call the new directory?");
        if (name is not null)
        {
            ViewModel.AddDirectoryCommand.Execute(name);
        }
    }

    private async void ViewModel_AddCollectionRequested(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Collection", "What would you like to call the new collection?");
        if (name is not null)
        {
            ViewModel.AddCollectionCommand.Execute(name);
        }
    }

    private async void ViewModel_AlertRequested(object sender, AlertEventArgs e)
    {
        await DisplayAlert(e.Title, e.Message, e.ConfirmButton);
    }

    private async void ViewModel_DeleteRequested(object sender, Components.Tree.ITreeItem e)
    {
        bool answer = await DisplayAlert("Delete Directory", $"This will delete the directory '{e.Name}' and its contents. Are you sure you want to delete it?", "Delete", "Cancel");
        if (answer)
        {
            ViewModel.DeleteCommand.Execute(e);
        }
    }
}

