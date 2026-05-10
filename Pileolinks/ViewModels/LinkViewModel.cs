using CommunityToolkit.Mvvm.Input;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using System.Collections.ObjectModel;

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

        public string UsedInformation
        {
            get
            {
                if (link.Used > 0 && link.LastUsed > DateOnly.MinValue)
                {
                    string numberText = link.Used switch
                    {
                        1 => $"Visited once on {link.LastUsed:d}",
                        2 => $"Visited twice; most recently on {link.LastUsed:d}",
                        _ => $"Visited {link.Used} times; most recently on {link.LastUsed:d}"
                    };

                    return numberText;
                }
                return string.Empty;
            }
        }

        public int Used => link.Used;

        public DateOnly LastUsed => link.LastUsed;

        public bool HasOpenParent => hasOpenParent;

        public LinkDirectory Parent => (LinkDirectory)link.Ancestor;

        public ObservableCollection<string> Tags { get; private set; } = [];

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
            UpdateLinkUsage();
        }

        [RelayCommand]
        private void AddTag(string tag)
        {
            if (!link.Tags.Contains(tag))
            {
                link.Tags.Add(tag);
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
            UpdateLinkUsage();
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

        private void UpdateLinkUsage()
        {
            link.Used++;
            link.LastUsed = DateOnly.FromDateTime(DateTime.Now);
            OnPropertyChanged(nameof(UsedInformation));
            SaveCollection();
        }

        public void Leaving()
        {
            SaveCollection();
        }
    }
}
