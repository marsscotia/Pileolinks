using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using Pileolinks.ViewModels.Enums;
using Pileolinks.ViewModels.Factories.Interfaces;
using System.Collections.ObjectModel;

namespace Pileolinks.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string SearchTerm { get; set; }

        [ObservableProperty]
        public partial string TagContent { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SearchHasNoResults))]
        [NotifyPropertyChangedFor(nameof(SearchHasResults))]
        [NotifyPropertyChangedFor(nameof(HasResultsAndIsNotLoading))]
        public partial bool SearchPerformed { get; set; }

        [ObservableProperty]
        public partial Sort SelectedSort { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasResultsAndIsNotLoading))]
        public partial bool Loading { get; set; }

        private readonly List<Link> links = [];

        private readonly List<Sort> sorts =
        [
            new Sort(SortType.Alphabetical, "A - Z"),
            new Sort(SortType.ReverseAlphabetical, "Z - A"),
            new Sort(SortType.MostVisited, "Most visited"),
            new Sort(SortType.LeastVisited, "Least visited"),
            new Sort(SortType.MostRecentlyVisited, "Most recently visited"),
            new Sort(SortType.LeastRecentlyVisited, "Least recently visited")
        ];

        private readonly IDataService dataService;
        private readonly ILinkViewModelFactory linkViewModelFactory;
        private readonly IEssentialsService essentialsService;
        private readonly ILinkCache linkCache;

        public bool HasResultsAndIsNotLoading => SearchHasResults && !Loading;

        public bool SearchHasNoResults => SearchPerformed && !Results.Any();

        public bool SearchHasResults => SearchPerformed && Results.Any();

        public ObservableCollection<string> SearchTags { get; private set; } = new();

        public ObservableCollection<LinkViewModel> Results { get; private set; } = new();

        public List<Sort> Sorts => sorts;

        public event EventHandler<DialogRequestedEventArgs> AlertRequested;
        public event EventHandler<string> LaunchUrlRequested;
        public event EventHandler<LinkViewModel> EditLinkRequested;
        public event EventHandler<LinkDirectory> OpenParentRequested;

        public SearchViewModel(IDataService dataService, ILinkViewModelFactory linkViewModelFactory, IEssentialsService essentialsService, ILinkCache linkCache) 
        {
            this.dataService = dataService;
            this.linkViewModelFactory = linkViewModelFactory;
            this.essentialsService = essentialsService;
            this.linkCache = linkCache;
            Results.CollectionChanged += Results_CollectionChanged;
            SearchPerformed = false;
            SelectedSort = sorts.FirstOrDefault(s => s.SortType == SortType.Alphabetical);
            Loading = false;
        }

        private void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SearchHasNoResults));
            OnPropertyChanged(nameof(SearchHasResults));
        }

        [RelayCommand]
        private async Task Search()
        {
            Loading = true;
            
            if (Results.Any())
            {
                foreach (var result in Results)
                {
                    result.EditLinkRequested -= LinkViewModel_EditLinkRequested;
                    result.LaunchUrlRequested -= LinkViewModel_LaunchUrlRequested;
                    result.OpenParentRequested -= LinkViewModel_OpenParentRequested;
                    result.CopyUrlRequested -= LinkViewModel_CopyUrlRequested;
                }
                Results.Clear();
            }
            
            var results = links.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                results = results.Where(l => (l.Name is not null && l.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                || (l.Description is not null &&  l.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                || (l.Uri is not null && l.Uri.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (SearchTags.Any())
            {
                results = results.Where(l => SearchTags.All(t => l.Tags.Contains(t)));
            }

            foreach (var link in results)
            {
                LinkViewModel linkViewModel = linkViewModelFactory.GetLinkViewModel(link, hasOpenParent: true);
                linkViewModel.LaunchUrlRequested += LinkViewModel_LaunchUrlRequested;
                linkViewModel.EditLinkRequested += LinkViewModel_EditLinkRequested;
                linkViewModel.CopyUrlRequested += LinkViewModel_CopyUrlRequested;
                linkViewModel.OpenParentRequested += LinkViewModel_OpenParentRequested;
                Results.Add(linkViewModel);
            }

            Loading = false;
            SearchPerformed = true;

            await Sort();
        }

        private void LinkViewModel_OpenParentRequested(object sender, EventArgs e)
        {
            OpenParentRequested?.Invoke(this, ((LinkViewModel)sender).Parent);
        }

        private async void LinkViewModel_CopyUrlRequested(object sender, string e)
        {
            try
            {
                await essentialsService.SetClipboardTextAsync(e);
                await essentialsService.ShowToastAsync("URL copied.");
            }
            catch (Exception)
            {
                await essentialsService.ShowToastAsync("We couldn't copy the URL. Please try again later.");
            }
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

        [RelayCommand]
        private async Task SearchOrAddTag()
        {
            if (!string.IsNullOrWhiteSpace(TagContent))
            {
                AddTag();
            }
            else
            {
                await Search();
            }
        }

        public void Arriving()
        {
            links.Clear();
            links.AddRange(linkCache.GetAllLinks());
        }

        public void Leaving()
        {
            if (Results.Any())
            {
                foreach (var result in Results)
                {
                    result.EditLinkRequested -= LinkViewModel_EditLinkRequested;
                    result.LaunchUrlRequested -= LinkViewModel_LaunchUrlRequested;
                    result.CopyUrlRequested -= LinkViewModel_CopyUrlRequested;
                    result.OpenParentRequested -= LinkViewModel_OpenParentRequested;
                }
            }
        }

        [RelayCommand]
        private async Task Sort()
        {
            if (SearchPerformed && SearchHasResults)
            {
                Loading = true;
                
                var sortedResults =  await Task.Run(() => SelectedSort.SortType switch
                {
                    SortType.Alphabetical => [.. Results.OrderBy(r => r.Name)],
                    SortType.ReverseAlphabetical => [.. Results.OrderByDescending(r => r.Name)],
                    SortType.MostVisited => [.. Results.OrderByDescending(r => r.Used).ThenByDescending(r => r.LastUsed)],
                    SortType.LeastVisited => [.. Results.OrderBy(r => r.Used).ThenByDescending(r => r.LastUsed)],
                    SortType.MostRecentlyVisited => [.. Results.OrderByDescending(r => r.LastUsed).ThenByDescending(r => r.Used)],
                    SortType.LeastRecentlyVisited => [.. Results.OrderBy(r => r.LastUsed).ThenByDescending(r => r.Used)],
                    _ => Results.ToList()
                });

                for (int i = 0; i < sortedResults.Count; i++)
                {
                    Results.Move(Results.IndexOf(sortedResults[i]), i);
                }

                Loading = false;
            }
        }
    }
}
