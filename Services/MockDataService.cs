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
            LinkDirectory recipes = new(Guid.NewGuid().ToString(), null, "Recipes", new());
            
            dev.AddDirectory("Windows");
            dev.AddDirectory("Web");

            recipes.AddDirectory("Curries");
            recipes.AddDirectory("Stir Fry");
            recipes.AddDirectory("Traditional");

            LinkDirectory trad = (LinkDirectory)recipes.GetDescendant("Traditional");
            trad.AddDirectory("Christmas");
            trad.AddDirectory("Sunday Dinner");

            treeItems.Add(dev);
            treeItems.Add(hols);
            treeItems.Add(recipes);

            return treeItems;
        }

        public bool SaveCollection(LinkDirectory linkDirectory)
        {
            throw new NotImplementedException();
        }

        public void SaveCollections(List<LinkDirectory> linkDirectories)
        {
            throw new NotImplementedException();
        }
    }
}
