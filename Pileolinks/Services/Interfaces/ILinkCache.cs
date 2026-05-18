using Pileolinks.Components.Tree;
using Pileolinks.Models;

namespace Pileolinks.Services.Interfaces
{
    /// <summary>
    /// Singleton cache that maintains a single in-memory copy of all Link and LinkDirectory objects.
    /// Ensures all ViewModels reference the same model instances for proper event propagation.
    /// </summary>
    public interface ILinkCache
    {
        /// <summary>
        /// Gets all top-level directories (collections).
        /// </summary>
        List<ITreeItem> GetTopLevelDirectories();

        /// <summary>
        /// Gets all links from all directories, recursively.
        /// </summary>
        List<Link> GetAllLinks();

        /// <summary>
        /// Explicitly refreshes the cache from persistent storage.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Indicates whether the cache has been populated.
        /// </summary>
        bool IsInitialized { get; }
    }
}
