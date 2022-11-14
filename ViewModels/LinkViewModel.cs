using CommunityToolkit.Mvvm.Input;
using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.ViewModels
{
    public partial class LinkViewModel : ItemViewModel
    {
        protected readonly Link link;

        public event EventHandler EditLinkRequested;
        public event EventHandler LaunchUrlRequested;

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

        public LinkViewModel(Link link) : base(link)
        {
            this.link = link;
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

    }
}
