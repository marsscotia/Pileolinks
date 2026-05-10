using System.Collections.ObjectModel;
using Pileolinks.Components.Tree;

namespace Pileolinks.Models
{
    public class Link(string id, ITreeItem parent, string uri = null, string name = null, HashSet<string> tags = default, int used = 0, DateOnly lastUsed = default) : ITreeItem
    {
        private readonly string id = id;
        private string name = name;
        private string description;
        private ITreeItem parent = parent;
        private string uri = uri;
        private int used = used;
        private DateOnly lastUsed = lastUsed;

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

        public int Used
        {
            get => used;
            set => used = value;
        }

        public DateOnly LastUsed
        {
            get => lastUsed;
            set => lastUsed = value;
        }

        public ObservableCollection<string> Tags { get; private set; } = new(tags ?? []);

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
            if (directory.Type != TreeItemType.Directory)
            {
                throw new ArgumentException("The specified tree item must be of type directory.");
            }

            bool result;

            if (HasAncestor)
            {
                Ancestor.Descendants.Remove(this);
            }
            directory.Descendants.Add(this);
            parent = directory;
            Moved?.Invoke(this, directory);
            result = true;

            return result;
        }

        public void Rename(string name)
        {
            throw new NotImplementedException();
        }

    }
}
