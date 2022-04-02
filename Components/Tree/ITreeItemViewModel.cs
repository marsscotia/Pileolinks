using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Components.Tree
{
    public interface ITreeItemViewModel 
    {
        ITreeItem TreeItem { get; }
        int Depth { get; }
        IReadOnlyCollection<ITreeItemViewModel> Descendants { get; }
        bool IsVisible { get; set; }
        string Id { get; }
        TreeItemType Type { get; }
        ITreeItem Ancestor { get; }
        bool HasAncestor { get; }
        bool HasDescendants { get; }
        bool HasDirectories { get; }
        string Name { get; }
        List<ITreeItem> Directories { get; }
        bool IsExpanded { get; set; }
        bool CanExpandAndIsExpanded { get; }
        bool CanExpandAndIsNotExpanded { get; }
        bool CannotExpand { get; }
        Command CollapseCommand { get; }
        Command ExpandCommand { get; }
        bool IsSelected { get; set; }
        bool IsNotSelected { get; }
        bool IsHovered { get; set; }

        event EventHandler<ITreeItemViewModel> DescendantAdded;
        event EventHandler Deleted;
        event EventHandler<ITreeItem> ItemMoved;

        void AddToDescendants(ITreeItemViewModel treeItemViewModel);
        void RecalculateDepth();
        void RemoveDescendant(ITreeItemViewModel descendant);
    }
}
