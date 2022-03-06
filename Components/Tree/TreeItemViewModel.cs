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
        private bool isVisible;
        private bool isExpanded;
        private int depth;
        private bool isSelected;
        private Command expandCommand;
        private Command collapseCommand;
        public string Id => treeItem.Id;

        public TreeItemType Type => treeItem.Type;

        public List<ITreeItemViewModel> Descendants { get; private set; } = new();

        public ITreeItem Ancestor => treeItem.Ancestor;

        public bool HasAncestor => treeItem.HasAncestor;

        public bool HasDescendants => treeItem.Descendants.Any();

        public bool HasDirectories => treeItem.HasDirectories;

        public string Name => treeItem.Name;

        public List<ITreeItem> Directories => treeItem.Directories;

        public ITreeItem TreeItem => treeItem;

        public Command ExpandCommand => expandCommand ??= new Command(Expand);
        public Command CollapseCommand => collapseCommand ??= new Command(Collapse);

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

        public bool IsNotSelected => !IsSelected;

        public bool CanExpandAndIsExpanded => HasDescendants && IsExpanded;
        public bool CanExpandAndIsNotExpanded => HasDescendants && !IsExpanded;
        public bool CannotExpand => !HasDescendants;

        public TreeItemViewModel(ITreeItem item)
        {
            treeItem = item;
            Depth = CalculateDepth();
            
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

    }
}
