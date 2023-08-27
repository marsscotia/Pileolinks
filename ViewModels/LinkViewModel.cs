using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public partial class LinkViewModel : ItemViewModel
    {
        protected readonly Link link;
        private string newTagContent;
        private readonly bool hasOpenParent;

        public event EventHandler EditLinkRequested;
        public event EventHandler LaunchUrlRequested;
        public event EventHandler DeleteLinkRequested;
        public event EventHandler<string> CopyUrlRequested;
        public event EventHandler OpenParentRequested;

        public Link Link => link;
        public string LinkUri
        {
            get => link?.Uri;
            set
            {
                if (link.Uri == null || !link.Uri.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    link.Uri = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => link?.Name;
            set
            {
                if (link.Name != null || !link.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    link.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => link?.Description;
            set
            {
                if (link.Description == null || !link.Description.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    link.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NewTagContent
        {
            get => newTagContent;
            set => SetProperty(ref newTagContent, value);
        }

        public bool HasOpenParent => hasOpenParent;

        public LinkDirectory Parent => (LinkDirectory)link.Ancestor;

        public ObservableCollection<string> Tags { get; private set; } = new();

        public LinkViewModel(IDataService dataService, Link link, bool hasOpenParent) : base(dataService, link)
        {
            this.link = link;
            this.hasOpenParent = hasOpenParent;
            foreach (var tag in link.Tags.OrderBy(t => t))
            {
                Tags.Add(tag);
            }
        }

        [RelayCommand]
        private void EditLink()
        {
            EditLinkRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void LaunchUrl()
        {
            LaunchUrlRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void AddTag(string tag)
        {
            bool success = link.Tags.Add(tag);
            if (success) 
            {
                Tags.Add(tag);
            }
            NewTagContent = string.Empty;
        }

        [RelayCommand]
        private void RemoveTag(string tag) 
        {
            bool success = link.Tags.Remove(tag);
            if (success) 
            {
                Tags.Remove(tag);
            }
        }

        [RelayCommand]
        private void RequestDelete()
        {
            DeleteLinkRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void RequestCopyUrl()
        {
            CopyUrlRequested?.Invoke(this, link.Uri);
        }

        [RelayCommand]
        private void RequestOpenParent()
        {
            OpenParentRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SaveCollection()
        {
            _ = dataService.SaveCollection(GetTopLevelAncestor());
        }

        public void Leaving()
        {
            SaveCollection();
        }
    }
}
