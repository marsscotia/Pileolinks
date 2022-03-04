using Pileolinks.Services.Interfaces;
using Pileolinks.Components.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pileolinks.Models;

namespace Pileolinks.Services
{
    internal class MockDataService : IDataService
    {
        public List<ITreeItem> GetTopLevelTreeItems()
        {
            List<ITreeItem> treeItems = new();

            LinkDirectory dev = new(Guid.NewGuid().ToString(), null, "Development", new());
            LinkDirectory hols = new(Guid.NewGuid().ToString(), null, "Holidays", new());
            dev.AddDirectory("Windows");
            dev.AddDirectory("Web");

            treeItems.Add(dev);
            treeItems.Add(hols);

            return treeItems;
        }
    }
}
