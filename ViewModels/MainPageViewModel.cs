using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services;
using System.Collections.ObjectModel;

namespace Pileolinks.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<ITreeItem> items = new();
        private ITreeItem selected;
        private Command requestDeleteDirectoryCommand, requestAddDirectoryCommand, requestAddCollectionCommand, requestRenameDirectoryCommand;
        private Command<ITreeItem> deleteDirectoryCommand;
        private Command<string> addDirectoryCommand, addCollectionCommand, renameDirectoryCommand;
        

        public event EventHandler<ITreeItem> DeleteRequested;
        public event EventHandler AddDirectoryRequested;
        public event EventHandler AddCollectionRequested;
        public event EventHandler RenameRequested;
        public event EventHandler<AlertEventArgs> AlertRequested;

        public ObservableCollection<ITreeItem> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public ITreeItem Selected
        {
            get => selected;
            set
            {
                SetProperty(ref selected, value);
                OnPropertyChanged(nameof(SelectedSummary));
                OnPropertyChanged(nameof(HasSelected));
            }
        }

        public ITreeItem InitialNavigate => Selected ?? Items.FirstOrDefault();

        public string SelectedSummary => Selected == null ?
                    "There is no currently selected directory" :
                    $"{Selected.Name} is the currently selected directory";

        public bool HasSelected => Selected != null;

        public Command<string> AddDirectoryCommand => addDirectoryCommand ??= new Command<string>(AddDirectory);
        public Command<string> AddCollectionCommand => addCollectionCommand ??= new Command<string>(AddCollection);
        public Command<ITreeItem> DeleteCommand => deleteDirectoryCommand ??= new Command<ITreeItem>(Delete);
        public Command RequestDeleteDirectoryCommand => requestDeleteDirectoryCommand ??= new Command(RequestDelete);
        public Command RequestAddDirectoryCommand => requestAddDirectoryCommand ??= new Command(RequestAddDirectory);
        public Command RequestAddCollectionCommand => requestAddCollectionCommand ??= new Command(RequestAddCollection);
        public Command RequestRenameDirectoryCommand => requestRenameDirectoryCommand ??= new Command(RequestRenameDirectory);
        public Command<string> RenameDirectoryCommand => renameDirectoryCommand ??= new Command<string>(RenameDirectory);

        public MainPageViewModel()
        {
            ObservableCollection<ITreeItem> items = new();
            foreach (ITreeItem item in new MockDataService().GetTopLevelTreeItems())
            {
                items.Add(item);
            }
            Items = items;
        }

        private void AddDirectory(string name)
        {
            if (Selected != null && !string.IsNullOrWhiteSpace(name))
            {
                bool valid = Selected.AddDirectory(name);
                if (!valid)
                {
                    AlertRequested?.Invoke(this, new AlertEventArgs
                    {
                        Title = "Directory Exists",
                        Message = "There's a directory here with that name already. Why not try another?",
                        ConfirmButton = "OK"
                    });
                }
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                AlertRequested.Invoke(this, new AlertEventArgs
                {
                    Title = "Missing Name",
                    Message = "Please supply a name for the directory",
                    ConfirmButton = "OK"
                });
            }
        }

        private void AddCollection(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!Items.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    LinkDirectory linkDirectory = new(Guid.NewGuid().ToString(), null, name, new List<ITreeItem>());
                    int count = 0;
                    ITreeItem found = null;
                    while(found == null && count < Items.Count)
                    {
                        if (string.Compare(name, Items.ElementAt(count).Name, StringComparison.OrdinalIgnoreCase) <= 0)
                        {
                            found = Items.ElementAt(count);
                        }
                        count++;
                    }
                    if (found == null)
                    {
                        Items.Add(linkDirectory);
                    }
                    else
                    {
                        Items.Insert(Items.IndexOf(found), linkDirectory);
                    }
                }
                else
                {
                    AlertRequested?.Invoke(this, new AlertEventArgs
                    {
                        Title = "Collection Exists",
                        Message = "There's a collection with that name here already. Why not try another?",
                        ConfirmButton = "OK"
                    });
                }
            }
            else
            {
                AlertRequested?.Invoke(this, new AlertEventArgs
                {
                    Title = "Missing Name",
                    Message = "Please supply a name for the collection",
                    ConfirmButton = "OK"
                });
            }
        }

        private void Delete(ITreeItem selected)
        {
            selected.Delete();
            if (selected.HasAncestor)
            {
                _ = selected.Ancestor.Descendants.Remove(selected);
            }
            else
            {
                _ = Items.Remove(selected);
            }
        }

        private void RequestDelete()
        {
            if (Selected is not null)
            {
                DeleteRequested?.Invoke(this, Selected);
            }
        }

        private void RequestAddDirectory()
        {
            if (Selected is not null)
            {
                AddDirectoryRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RequestRenameDirectory()
        {
            if (Selected is not null)
            {
                RenameRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RequestAddCollection()
        {
            AddCollectionRequested?.Invoke(this, EventArgs.Empty);
        }

        private void RenameDirectory(string name)
        {
            if (Selected.HasAncestor)
            {
                if (Selected.Ancestor.Directories.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    AlertRequested?.Invoke(this, new AlertEventArgs
                    {
                        Title = "Same Name",
                        Message = "There's already a directory with that name here. Why not try another?",
                        ConfirmButton = "OK"
                    });
                }
                else
                {
                    Selected.Rename(name);
                }
            }
            else
            {
                if (Items.Any(i => i.Name.Equals(name)))
                {
                    AlertRequested?.Invoke(this, new AlertEventArgs
                    {
                        Title = "Same Name",
                        Message = "There's already a directory with that name here. Why not try another?",
                        ConfirmButton = "OK"
                    });
                }
                else
                {
                    Selected.Rename(name);
                }
            }
        }

    }
}
