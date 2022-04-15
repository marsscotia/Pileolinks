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
        private readonly string id;
        private string name;
        private string description;
        private ITreeItem parent;
        private string uri;

        public string Id => id;

        public string Name
        {
            get => name ?? (uri?.ToString() ?? string.Empty);
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string Uri
        {
            get => uri;
            set => uri = value;
        }

        public TreeItemType Type => TreeItemType.Link;

        public List<ITreeItem> Descendants => null;

        public List<ITreeItem> Directories => null;

        public ITreeItem Ancestor => parent;

        public bool HasAncestor => parent != null;

        public bool HasDescendants => false;

        public bool HasDirectories => false;

        public event EventHandler<ITreeItem> DescendantAdded;
        public event EventHandler Deleted;
        public event EventHandler<ITreeItem> Moved;
        public event EventHandler NameChanged;

        public Link(string id, ITreeItem parent, string uri = null, string name = null)
        {
            this.id = id;
            this.parent = parent;
            this.uri = uri;
            this.name = name;
        }

        

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
