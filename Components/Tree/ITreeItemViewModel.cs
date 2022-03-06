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
        List<ITreeItemViewModel> Descendants { get; }
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
    }
}
