using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pileolinks.ViewModels.Enums;
using System.Collections.ObjectModel;

namespace Pileolinks.ViewModels.Behaviors
{
    /// <summary>
    /// Generic behavior that provides reusable sort functionality for any collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items being sorted</typeparam>
    public partial class SortableBehavior<T> : ObservableObject
    {
        [ObservableProperty]
        public partial Sort SelectedSort { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasItems))]
        public partial bool IsLoading { get; set; }

        [ObservableProperty]
        public partial bool HasItems { get; set; }

        private readonly List<Sort> sorts =
        [
            new Sort(SortType.Alphabetical, "A - Z"),
            new Sort(SortType.ReverseAlphabetical, "Z - A"),
            new Sort(SortType.MostVisited, "Most visited"),
            new Sort(SortType.LeastVisited, "Least visited"),
            new Sort(SortType.MostRecentlyVisited, "Most recently visited"),
            new Sort(SortType.LeastRecentlyVisited, "Least recently visited")
        ];

        private readonly ObservableCollection<T> items;
        private readonly Func<ObservableCollection<T>, Sort, Task<List<T>>> sortLogic;

        public List<Sort> Sorts => sorts;

        /// <summary>
        /// Initializes a new instance of SortableBehavior.
        /// </summary>
        /// <param name="items">The collection to sort</param>
        /// <param name="sortLogic">Delegate that defines how to sort items based on the selected Sort</param>
        public SortableBehavior(
            ObservableCollection<T> items,
            Func<ObservableCollection<T>, Sort, Task<List<T>>> sortLogic)
        {
            this.items = items;
            this.sortLogic = sortLogic;
            SelectedSort = sorts.FirstOrDefault(s => s.SortType == SortType.Alphabetical);
            IsLoading = false;
            HasItems = items.Any();

            items.CollectionChanged += (s, e) =>
            {
                HasItems = items.Any();
            };
        }

        [RelayCommand]
        public async Task Sort()
        {
            if (!HasItems)
                return;

            IsLoading = true;

            try
            {
                var sortedResults = await sortLogic(items, SelectedSort);

                for (int i = 0; i < sortedResults.Count; i++)
                {
                    var item = sortedResults[i];
                    var currentIndex = items.IndexOf(item);
                    if (currentIndex != i)
                    {
                        items.Move(currentIndex, i);
                    }
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Resets the selected sort to the default alphabetical sort.
        /// </summary>
        public void ResetSort()
        {
            SelectedSort = sorts.FirstOrDefault(s => s.SortType == SortType.Alphabetical);
        }
    }
}
