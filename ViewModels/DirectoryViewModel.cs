using Pileolinks.Components.Tree;
using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    
    public class DirectoryViewModel : ItemViewModel
    {
        protected readonly LinkDirectory directory;
        private Command<DirectoryViewModel> selectDirectoryCommand;
        private Command addLinkCommand;
        private Command<string> addDirectoryCommand;

        public ObservableCollection<ItemViewModel> Items { get; private set; } = new ObservableCollection<ItemViewModel>();
        public Command<DirectoryViewModel> SelectDirectoryCommand => selectDirectoryCommand ??= new Command<DirectoryViewModel>(SelectDirectory);
        public Command AddLinkCommand => addLinkCommand ??= new Command(AddLink);
        public Command<string> AddDirectoryCommand => addDirectoryCommand ??= new Command<string>(AddDirectory);
        public LinkDirectory LinkDirectory => directory;
        public string Name => item?.Name;

        public event EventHandler<DirectoryViewModel> DirectorySelected;
        public event EventHandler<LinkViewModel> LinkAdded;
        public event EventHandler<AlertEventArgs> AlertRequested;
        
        public DirectoryViewModel(LinkDirectory directory) : base(directory)
        {
            this.directory = directory;
            if (directory != null)
            {
                directory.DescendantAdded += Directory_DescendantAdded; 
            }
        }

        private void Directory_DescendantAdded(object sender, ITreeItem e)
        {
            InsertIntoDescendants(new DirectoryViewModel((LinkDirectory)e));
        }

        public void Initialise()
        {
            PopulateItems();
        }

        private void PopulateItems()
        {
            Items.Clear();
            if (directory != null)
            {
                foreach (var item in directory.Directories.OrderBy(d => d.Name))
                {
                    Items.Add(new DirectoryViewModel((LinkDirectory)item));
                }
                foreach (var item in directory.Descendants.Where(d => d.Type != TreeItemType.Directory).OrderBy(d => d.Name))
                {
                    Items.Add(new LinkViewModel((Link)item));
                } 
            }
        }

        private void SelectDirectory(DirectoryViewModel directoryViewModel)
        {
            DirectorySelected?.Invoke(this, directoryViewModel);
        }

        private void AddLink()
        {
            Link link = directory.AddLink();
            LinkViewModel linkViewModel = new(link);
            LinkAdded?.Invoke(this, linkViewModel);
        }

        private void AddDirectory(string name)
        {
            if(!string.IsNullOrWhiteSpace(name))
            {
                bool valid = directory.AddDirectory(name);
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
            else
            {
                AlertRequested?.Invoke(this, new AlertEventArgs
                {
                    Title = "Missing Name",
                    Message = "Please supply a name for the directory",
                    ConfirmButton = "OK"
                });
            }
        }

        private void InsertIntoDescendants(DirectoryViewModel directoryViewModel)
        {
            if (!Items.Any())
            {
                Items.Add(directoryViewModel);
            }
            else if (!Items.Any(i => i.Type == TreeItemType.Directory))
            {
                Items.Insert(0, directoryViewModel);
            }
            else
            {
                var directories = Items.Where(i => i.Type == TreeItemType.Directory).Select(i => (DirectoryViewModel)i);
                DirectoryViewModel found = null;
                int counter = 0;
                while (counter < directories.Count() && found == null)
                {
                    if (string.Compare(directoryViewModel.Name, directories.ElementAt(counter).Name, StringComparison.OrdinalIgnoreCase) <= 0) 
                    {
                        found = directories.ElementAt(counter);
                    }
                    counter++;
                }
                if (found != null)
                {
                    Items.Insert(Items.IndexOf(found), directoryViewModel);
                }
                else
                {
                    int maxIndex = directories.Select(d => Items.IndexOf(d)).Max();
                    Items.Insert(maxIndex + 1, directoryViewModel);
                }
            }
        }

    }
}
