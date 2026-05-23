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
        ITreeItem Ancestor { get; }
        bool HasAncestor { get; }

        event EventHandler Deleted;
        event EventHandler NameChanged;

        void Delete();
        void Rename(string name);
        bool MoveToDirectory(ITreeItem directory);
    }

    public interface IParentTreeItem : ITreeItem
    {
        List<ITreeItem> Descendants { get; }
        List<ITreeItem> Directories { get; }
        bool HasDescendants { get; }
        bool HasDirectories { get; }

        event EventHandler<ITreeItem> DescendantAdded;
        event EventHandler<ITreeItem> Moved;

        bool AddDirectory(string name);
    }
}
