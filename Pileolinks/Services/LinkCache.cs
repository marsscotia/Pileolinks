using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;

namespace Pileolinks.Services
{
    /// <summary>
    /// Singleton cache service that maintains a single in-memory copy of all Link and LinkDirectory objects.
    /// All ViewModels reference the same model instances through this cache, ensuring that updates
    /// (like link usage changes) are visible across all pages without needing to reload from disk.
    /// </summary>
    public class LinkCache : ILinkCache
    {
        private readonly IDataService _dataService;
        private List<ITreeItem> _topLevelDirectories = [];
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;

        public LinkCache(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Gets all top-level directories. Lazy-loads from persistent storage on first access.
        /// </summary>
        public List<ITreeItem> GetTopLevelDirectories()
        {
            if (!_isInitialized)
            {
                Refresh();
            }
            return _topLevelDirectories;
        }

        /// <summary>
        /// Gets all links from all directories, recursively.
        /// Uses a stack-based traversal to walk the entire directory tree.
        /// </summary>
        public List<Link> GetAllLinks()
        {
            List<Link> allLinks = [];
            var topLevelDirectories = GetTopLevelDirectories();

            Stack<ITreeItem> stack = new();
            foreach (var directory in topLevelDirectories)
            {
                stack.Push(directory);
            }

            while (stack.Count > 0)
            {
                LinkDirectory directory = (LinkDirectory)stack.Pop();
                allLinks.AddRange(directory.Descendants.Where(d => d.Type == TreeItemType.Link).Select(l => (Link)l));
                foreach (var item in directory.Directories)
                {
                    stack.Push(item);
                }
            }

            return allLinks;
        }

        /// <summary>
        /// Explicitly refreshes the cache from persistent storage.
        /// Called on first access or can be called manually to reload all data.
        /// </summary>
        public void Refresh()
        {
            _topLevelDirectories = _dataService.GetTopLevelTreeItems();
            _isInitialized = true;
        }
    }
}
