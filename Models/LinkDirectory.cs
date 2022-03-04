using Pileolinks.Components.Tree;

namespace Pileolinks.Models
{
    internal class LinkDirectory : ITreeItem
    {
        private readonly string id;
        private readonly ITreeItem parent;

        public string Id => id;

        public string Name { get; set; }

        public TreeItemType Type => TreeItemType.Directory;

        public List<ITreeItem> Descendants { get; } = new();

        public List<ITreeItem> Directories => Descendants.Where(d => d.Type == TreeItemType.Directory).ToList();

        public ITreeItem Ancestor => parent;

        public bool HasAncestor => Ancestor != null;

        public bool HasDescendants => Descendants != null && Descendants.Any();

        public bool HasDirectories => Descendants != null && Descendants.Any(d => d.Type == TreeItemType.Directory);

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
            }

            return result;
        }

        public ITreeItem GetDescendant(string name)
        {
            return Descendants.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

    }
}
