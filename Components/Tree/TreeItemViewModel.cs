using Pileolinks.Components;
using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Components.Tree
{
    internal class TreeItemViewModel : BaseViewModel, ITreeItemViewModel
    {
        private readonly ITreeItem treeItem;
        private readonly List<ITreeItemViewModel> descendants = new();
        private bool isVisible;
        private bool isExpanded;
        private int depth;
        private bool isSelected;
        private bool isHovered;
        private Command expandCommand;
        private Command collapseCommand;
        public string Id => treeItem.Id;

        public TreeItemType Type => treeItem.Type;

        public IReadOnlyCollection<ITreeItemViewModel> Descendants { get; private set; }

        public ITreeItem Ancestor => treeItem.Ancestor;

        public bool HasAncestor => treeItem.HasAncestor;

        public bool HasDescendants => Descendants.Any();

        public bool HasDirectories => treeItem.HasDirectories;

        public string Name => treeItem.Name;

        public List<ITreeItem> Directories => treeItem.Directories;

        public ITreeItem TreeItem => treeItem;

        public Command ExpandCommand => expandCommand ??= new Command(Expand);
        public Command CollapseCommand => collapseCommand ??= new Command(Collapse);

        public event EventHandler<ITreeItemViewModel> DescendantAdded;
        public event EventHandler Deleted;
        public event EventHandler<ITreeItem> ItemMoved;

        public int Depth
        {
            get => depth;
            private set => SetProperty(ref depth, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                bool changed = SetProperty(ref isVisible, value);
                if (changed && HasDescendants && IsExpanded)
                {
                    foreach (ITreeItemViewModel item in Descendants)
                    {
                        item.IsVisible = value;
                    }
                }
            }
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                bool changed = SetProperty(ref isExpanded, value);
                if (changed)
                {
                    OnPropertyChanged(nameof(CanExpandAndIsExpanded));
                    OnPropertyChanged(nameof(CanExpandAndIsNotExpanded));

                    if (HasDescendants)
                    {
                        foreach (ITreeItemViewModel item in Descendants)
                        {
                            item.IsVisible = value;
                        } 
                    }
                }
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                SetProperty(ref isSelected, value);
                OnPropertyChanged(nameof(IsNotSelected));
            }
        }

        public bool IsHovered
        {
            get => isHovered;
            set => SetProperty(ref isHovered, value);
        }

        public bool IsNotSelected => !IsSelected;

        public bool CanExpandAndIsExpanded => HasDescendants && IsExpanded;
        public bool CanExpandAndIsNotExpanded => HasDescendants && !IsExpanded;
        public bool CannotExpand => !HasDescendants;

        public TreeItemViewModel(ITreeItem item)
        {
            treeItem = item;
            item.DescendantAdded += AddDescendant;
            item.Deleted += Delete;
            item.Moved += MoveItem;
            Depth = CalculateDepth();
            Descendants = descendants.AsReadOnly();
        }

        private int CalculateDepth()
        {
            int depth = 0;

            ITreeItem ancestor = treeItem;

            while (ancestor.HasAncestor)
            {
                depth++;
                ancestor = ancestor.Ancestor;
            }

            return depth;
        }

        private void Expand()
        {
            IsExpanded = true;
        }

        private void Collapse()
        {
            IsExpanded = false;
        }

        private void AddDescendant(object sender, ITreeItem descendant)
        {
            TreeItemViewModel treeItemViewModel = new(descendant);
            InsertIntoDescendants(treeItemViewModel);

            IsExpanded = true;
            DescendantAdded?.Invoke(this, treeItemViewModel);
        }

        private void Delete(object sender, EventArgs e)
        {
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        public void AddToDescendants(ITreeItemViewModel treeItemViewModel)
        {
            InsertIntoDescendants(treeItemViewModel);
        }

        private void InsertIntoDescendants(ITreeItemViewModel treeItemViewModel)
        {
            int count = 0;
            ITreeItemViewModel found = null;

            while (count < Descendants.Count && found == null)
            {
                if (string.Compare(treeItemViewModel.Name, Descendants.ElementAt(count).Name, StringComparison.OrdinalIgnoreCase) <= 0)
                {
                    found = Descendants.ElementAt(count);
                }
                count++;
            }
            if (found != null)
            {
                descendants.Insert(descendants.IndexOf(found), treeItemViewModel);
            }
            else
            {
                descendants.Add(treeItemViewModel);
            }

            OnPropertyChanged(nameof(HasDescendants));
            OnPropertyChanged(nameof(CanExpandAndIsExpanded));
            OnPropertyChanged(nameof(CanExpandAndIsNotExpanded));
            OnPropertyChanged(nameof(CannotExpand));
        }

        private void MoveItem(object sender, ITreeItem newAncestor)
        {
            RecalculateDepth();
            ItemMoved?.Invoke(this, newAncestor);
        }

        public void RecalculateDepth()
        {
            Depth = CalculateDepth();
            foreach (var item in Descendants)
            {
                item.RecalculateDepth();
            }
        }

        public void RemoveDescendant(ITreeItemViewModel descendant)
        {
            descendants.Remove(descendant);
            if (!descendants.Any())
            {
                IsExpanded = false;
            }
            OnPropertyChanged(nameof(HasDescendants));
            OnPropertyChanged(nameof(CanExpandAndIsExpanded));
            OnPropertyChanged(nameof(CanExpandAndIsNotExpanded));
            OnPropertyChanged(nameof(CannotExpand));
        }

    }
}
