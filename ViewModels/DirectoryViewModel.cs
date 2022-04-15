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

        public ObservableCollection<ItemViewModel> Items { get; private set; } = new ObservableCollection<ItemViewModel>();
        public Command<DirectoryViewModel> SelectDirectoryCommand => selectDirectoryCommand ??= new Command<DirectoryViewModel>(SelectDirectory);
        public Command AddLinkCommand => addLinkCommand ??= new Command(AddLink);
        public LinkDirectory LinkDirectory => directory;
        public string Name => item?.Name;

        public event EventHandler<DirectoryViewModel> DirectorySelected;
        public event EventHandler<LinkViewModel> LinkAdded;
        
        public DirectoryViewModel(LinkDirectory directory) : base(directory)
        {
            this.directory = directory;
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

    }
}
