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
        private bool searchPerformed;

        private readonly List<LinkViewModel> links = new();

        private readonly IDataService dataService;

        public bool SearchHasResults => SearchPerformed && Results.Any();

        public ObservableCollection<string> SearchTags { get; private set; }

        public ObservableCollection<LinkViewModel> Results { get; private set; } = new();

        public event EventHandler<DialogRequestedEventArgs> AlertRequested;

        public SearchViewModel(IDataService dataService) 
        {
            this.dataService = dataService;
            Results.CollectionChanged += Results_CollectionChanged;
        }

        private void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SearchHasResults));
        }

        [RelayCommand]
        private void Search()
        {
            Results.Clear();
            
            var results = links.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                results = results.Where(l => l.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                || l.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                || l.LinkUri.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (SearchTags.Any())
            {
                results = results.Where(l => l.Tags.All(t => l.Tags.Contains(t)));
            }

            foreach (var link in results)
            {
                Results.Add(link);
            }

            SearchPerformed = true;
            OnPropertyChanged(nameof(SearchHasResults));
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchTags.Clear();
            SearchTerm = string.Empty;
            Results.Clear();
            SearchPerformed = false;
            OnPropertyChanged(nameof(SearchHasResults));
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
                links.AddRange(directory.Descendants.Where(d => d.Type == TreeItemType.Link).Select(l => new LinkViewModel((Link)l)));
                foreach (var item in directory.Directories)
                {
                    stack.Push(item);
                }
            }
        }
    }
}
