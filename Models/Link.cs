using Pileolinks.Components.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Models
{
    public class Link : ITreeItem
    {
        public string Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public TreeItemType Type => TreeItemType.Link;

        public List<ITreeItem> Descendants => throw new NotImplementedException();

        public List<ITreeItem> Directories => throw new NotImplementedException();

        public ITreeItem Ancestor => throw new NotImplementedException();

        public bool HasAncestor => throw new NotImplementedException();

        public bool HasDescendants => throw new NotImplementedException();

        public bool HasDirectories => throw new NotImplementedException();

        public event EventHandler<ITreeItem> DescendantAdded;
        public event EventHandler Deleted;
        public event EventHandler<ITreeItem> Moved;
        public event EventHandler NameChanged;

        public bool AddDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool MoveToDirectory(ITreeItem directory)
        {
            throw new NotImplementedException();
        }

        public void Rename(string name)
        {
            throw new NotImplementedException();
        }
    }
}
