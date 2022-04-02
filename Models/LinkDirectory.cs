using Pileolinks.Components.Tree;

namespace Pileolinks.Models
{
    internal class LinkDirectory : ITreeItem
    {
        private readonly string id;
        private ITreeItem parent;
        private string name = string.Empty;

        public string Id => id;

        public string Name
        {
            get => name;
            set
            {
                if (!name.Equals(value))
                {
                    name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TreeItemType Type => TreeItemType.Directory;

        public List<ITreeItem> Descendants { get; } = new();

        public List<ITreeItem> Directories => Descendants.Where(d => d.Type == TreeItemType.Directory).ToList();

        public ITreeItem Ancestor => parent;

        public bool HasAncestor => Ancestor != null;

        public bool HasDescendants => Descendants != null && Descendants.Any();

        public bool HasDirectories => Descendants != null && Descendants.Any(d => d.Type == TreeItemType.Directory);

        public event EventHandler<ITreeItem> DescendantAdded;
        public event EventHandler Deleted;
        public event EventHandler<ITreeItem> Moved;
        public event EventHandler NameChanged;

        public LinkDirectory(string id, ITreeItem parent, string name, List<ITreeItem> descendants)
        {
            this.id = id;
            this.parent = parent;
            Name = name;
            Descendants = descendants;
        }

        public bool AddDirectory(string name)
        {
            bool result;

            if (Descendants.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result = false;
            }
            else
            {
                LinkDirectory linkDirectory = new(Guid.NewGuid().ToString(), this, name, new());
                Descendants.Add(linkDirectory);
                result = true;
                DescendantAdded?.Invoke(this, linkDirectory);
            }

            return result;
        }

        public bool MoveToDirectory(ITreeItem directory)
        {
            if (directory.Type != TreeItemType.Directory)
            {
                throw new ArgumentException("The specified directory must be of type directory");
            }

            bool result;

            if (directory.Descendants.Any(d => d.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)))
            {
                result = false;
            }
            else
            {
                if (HasAncestor)
                {
                    Ancestor.Descendants.Remove(this); 
                }
                directory.Descendants.Add(this);
                parent = directory;
                Moved?.Invoke(this, directory);
                result = true;
            }

            return result;
        }

        public ITreeItem GetDescendant(string name)
        {
            return Descendants.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Delete()
        {
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        public void Rename(string name)
        {
            Name = name;
        }
    }
}
