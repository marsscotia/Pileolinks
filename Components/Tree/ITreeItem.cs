using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Components.Tree
{
    public enum TreeItemType
    {
        Directory = 0,
        Link = 1
    }

    public interface ITreeItem
    {
        string Id { get; }
        string Name { get; }
        TreeItemType Type { get; }
        List<ITreeItem> Descendants { get; }
        List<ITreeItem> Directories { get; }
        ITreeItem Ancestor { get; }
        bool HasAncestor { get; }
        bool HasDescendants { get; }
        bool HasDirectories { get; }
    }
}
