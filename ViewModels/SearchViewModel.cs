using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Pileolinks.ViewModels
{
    [INotifyPropertyChanged]
    public partial class SearchViewModel
    {
        [ObservableProperty]
        private string searchTerm;

        [ObservableProperty]
        private string tagContent;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SearchHasNoResults))]
        [NotifyPropertyChangedFor(nameof(SearchHasResults))]
        private bool searchPerformed;

        private readonly List<Link> links = new();

        private readonly IDataService dataService;

        public bool SearchHasNoResults => SearchPerformed && !Results.Any();

        public bool SearchHasResults => SearchPerformed && Results.Any();

        public ObservableCollection<string> SearchTags { get; private set; } = new();

        public ObservableCollection<LinkViewModel> Results { get; private set; } = new();

        public event EventHandler<DialogRequestedEventArgs> AlertRequested;
        public event EventHandler<string> LaunchUrlRequested;
        public event EventHandler<LinkViewModel> EditLinkRequested;

        public SearchViewModel(IDataService dataService) 
        {
            this.dataService = dataService;
            Results.CollectionChanged += Results_CollectionChanged;
            SearchPerformed = false;
        }

        private void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SearchHasNoResults));
            OnPropertyChanged(nameof(SearchHasResults));
        }

        [RelayCommand]
        private void Search()
        {
            if (Results.Any())
            {
                foreach (var result in Results)
                {
                    result.EditLinkRequested -= LinkViewModel_EditLinkRequested;
                    result.LaunchUrlRequested -= LinkViewModel_LaunchUrlRequested;
                }
                Results.Clear();
            }
            
            var results = links.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                results = results.Where(l => l.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                || l.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                || l.Uri.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (SearchTags.Any())
            {
                results = results.Where(l => SearchTags.All(t => l.Tags.Contains(t)));
            }

            foreach (var link in results)
            {
                LinkViewModel linkViewModel = new(link);
                linkViewModel.LaunchUrlRequested += LinkViewModel_LaunchUrlRequested;
                linkViewModel.EditLinkRequested += LinkViewModel_EditLinkRequested;
                Results.Add(linkViewModel);
            }

            SearchPerformed = true;
        }

        private void LinkViewModel_EditLinkRequested(object sender, EventArgs e)
        {
            EditLinkRequested?.Invoke(this, (LinkViewModel)sender);
        }

        private void LinkViewModel_LaunchUrlRequested(object sender, EventArgs e)
        {
            LaunchUrlRequested?.Invoke(this, ((LinkViewModel)sender).LinkUri);
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchTags.Clear();
            SearchTerm = string.Empty;
            foreach (var result in Results)
            {
                result.LaunchUrlRequested -= LinkViewModel_LaunchUrlRequested;
                result.EditLinkRequested -= LinkViewModel_EditLinkRequested;
            }
            Results.Clear();
            SearchPerformed = false;
        }

        [RelayCommand]
        private void AddTag()
        {
            if (!SearchTags.Any(t => t.Equals(TagContent, StringComparison.OrdinalIgnoreCase)))
            {
                SearchTags.Add(TagContent);
                TagContent = string.Empty;
            }
            else
            {
                AlertRequested?.Invoke(this, new("Duplicate tag", "This tag already exists in your search tags. Why not try another?", "OK"));
            }
        }

        [RelayCommand]
        private void RemoveTag(string tagContent)
        {
            SearchTags.Remove(tagContent);
        }

        public void Arriving()
        {
            List<ITreeItem> collections = dataService.GetTopLevelTreeItems();
            Stack<ITreeItem> stack = new();
            foreach (var collection in collections) 
            {
                stack.Push(collection);
            }
            while (stack.Count > 0)
            {
                LinkDirectory directory = (LinkDirectory)stack.Pop();
                links.AddRange(directory.Descendants.Where(d => d.Type == TreeItemType.Link).Select(l => (Link)l));
                foreach (var item in directory.Directories)
                {
                    stack.Push(item);
                }
            }
        }

        public void Leaving()
        {
            if (Results.Any())
            {
                foreach (var result in Results)
                {
                    result.EditLinkRequested -= LinkViewModel_EditLinkRequested;
                    result.LaunchUrlRequested -= LinkViewModel_LaunchUrlRequested;
                }
            }
        }
    }
}
