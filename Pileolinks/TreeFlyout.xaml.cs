using Pileolinks.Components.Tree;
using Pileolinks.ViewModels;

namespace Pileolinks;

public partial class TreeFlyout : ContentPage
{
    public MainPageViewModel ViewModel { get; private set; }
    public event EventHandler<ITreeItem> TreeItemViewModelChanged;

	public TreeFlyout(MainPageViewModel mainPageViewModel)
	{
		InitializeComponent();
        BindingContext = ViewModel = mainPageViewModel;
        ViewModel.DeleteRequested += ViewModel_DeleteRequested;
        ViewModel.AlertRequested += ViewModel_AlertRequested;
        ViewModel.AddCollectionRequested += ViewModel_AddCollectionRequested;
        ViewModel.AddDirectoryRequested += ViewModel_AddDirectoryRequested;
        ViewModel.RenameRequested += ViewModel_RenameRequested;
    }

    private void Tree_SelectedItemChanged(object sender, ITreeItem e)
    {
		TreeItemViewModelChanged?.Invoke(this, e);
    }

    private async void ViewModel_RenameRequested(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Name", "What would you like to call the directory?");
        if (name is not null)
        {
            ViewModel.RenameDirectoryCommand.Execute(name);
        }
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

    private async void Tree_InvalidMoveRequested(object sender, string e)
    {
        await DisplayAlert("Impossible Move", e, "OK");
    }

}