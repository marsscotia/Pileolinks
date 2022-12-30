using CommunityToolkit.Mvvm.Input;
using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using Pileolinks.ViewModels.Factories.Interfaces;
using Pileolinks.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    
    public partial class DirectoryViewModel : ItemViewModel
    {
        protected readonly LinkDirectory directory;
        private Command<string> addDirectoryCommand;

        public ObservableCollection<ItemViewModel> Items { get; private set; } = new ObservableCollection<ItemViewModel>();
        
        public Command<string> AddDirectoryCommand => addDirectoryCommand ??= new Command<string>(AddDirectory);
        public LinkDirectory LinkDirectory => directory;
        public string Name => item?.Name;

        public event EventHandler<DirectoryViewModel> DirectorySelected;
        public event EventHandler<LinkViewModel> EditLinkRequested;
        public event EventHandler<AlertEventArgs> AlertRequested;
        public event EventHandler<string> LaunchUrlRequested;
        public event EventHandler SearchRequested;
        public event EventHandler<RequestDeleteLinkEventArgs> ConfirmDeleteLinkRequested;
        
        public DirectoryViewModel(IDataService dataService, LinkDirectory directory) : base(dataService, directory)
        {
            this.directory = directory;
            if (directory != null)
            {
                directory.DescendantAdded += Directory_DescendantAdded; 
            }
        }

        private void Directory_DescendantAdded(object sender, ITreeItem e)
        {
            DirectoryViewModel directoryViewModel = new(dataService, (LinkDirectory)e);
            directoryViewModel.DirectorySelected += DescendantDirectorySelected;
            InsertIntoDescendants(directoryViewModel);
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
                    DirectoryViewModel directoryViewModel = new(dataService, (LinkDirectory)item);
                    directoryViewModel.DirectorySelected += DescendantDirectorySelected;
                    Items.Add(directoryViewModel);
                }
                foreach (var item in directory.Descendants.Where(d => d.Type != TreeItemType.Directory).OrderBy(d => d.Name))
                {
                    LinkViewModel linkViewModel = new(dataService, (Link)item);
                    linkViewModel.EditLinkRequested += EditLink;
                    linkViewModel.LaunchUrlRequested += LaunchUrl;
                    linkViewModel.DeleteLinkRequested += DeleteLinkRequested;
                    Items.Add(linkViewModel);
                } 
            }
        }

        [RelayCommand]
        private void SelectDirectory(DirectoryViewModel directoryViewModel)
        {
            DirectorySelected?.Invoke(this, directoryViewModel);
        }

        private void DescendantDirectorySelected(object sender, DirectoryViewModel selected)
        {
            DirectorySelected?.Invoke(this, selected);
        }

        private void EditLink(object sender, EventArgs e)
        {
            EditLinkRequested?.Invoke(this, (LinkViewModel)sender);
        }

        private void LaunchUrl(object sender, EventArgs e)
        {
            LaunchUrlRequested?.Invoke(this, ((LinkViewModel)sender).LinkUri);
        }

        private void DeleteLinkRequested(object sender, EventArgs e)
        {
            LinkViewModel linkViewModel = (LinkViewModel)sender;
            ConfirmDeleteLinkRequested?.Invoke(this, new RequestDeleteLinkEventArgs
            {
                Title = "Are you sure?",
                Message = $"Do you really want to delete {linkViewModel.Name}?",
                ConfirmButton = "Delete",
                CancelButton = "Cancel",
                Id = linkViewModel.Item.Id,
                Name = linkViewModel.Name
            });
        }

        [RelayCommand]
        private void DeleteLink(string id)
        {
            LinkViewModel linkViewModel = (LinkViewModel)Items.First(i => i.Item.Id == id);
            linkViewModel.EditLinkRequested -= EditLink;
            linkViewModel.DeleteLinkRequested -= DeleteLinkRequested;
            linkViewModel.LaunchUrlRequested -= LaunchUrl;
            Items.Remove(linkViewModel);
            _ = directory.Descendants.Remove(linkViewModel.Link);
            SaveCollection();
        }

        [RelayCommand]
        private void AddLink()
        {
            Link link = directory.AddLink();
            LinkViewModel linkViewModel = new(dataService, link);
            EditLinkRequested?.Invoke(this, linkViewModel);
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
        
        [RelayCommand]
        private void Search()
        {
            SearchRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SaveCollection()
        {
            if (!NoItem)
            {
                _ = dataService.SaveCollection(GetTopLevelAncestor()); 
            }
        }

        public void Leaving()
        {
            SaveCollection();
        }

    }
}
