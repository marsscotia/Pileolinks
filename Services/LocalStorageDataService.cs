using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pileolinks.Services
{
    public class LocalStorageDataService : IDataService
    {
        private static readonly string FilePrefix = "Pileolinks-";
        private readonly IDataConverter _converter;

        public LocalStorageDataService(IDataConverter converter)
        {
            _converter = converter;
        }

        public List<ITreeItem> GetTopLevelTreeItems()
        {
            List<ITreeItem> results = new();
            List<ITreeItemDTO> treeItems = new();
            var files = Directory.EnumerateFiles(FileSystem.Current.AppDataDirectory);
            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).StartsWith(FilePrefix))
                {
                    string jsonText = File.ReadAllText(file);
                    try
                    {
                        List<ITreeItemDTO> current = JsonSerializer.Deserialize<List<ITreeItemDTO>>(jsonText);
                        treeItems.AddRange(current);
                    }
                    catch (Exception)
                    {
                        File.Delete(file);
                    }
                }
            }
            results.AddRange(_converter.GetTreeItems(treeItems));
            return results;
        }

        public bool SaveCollection(LinkDirectory linkDirectory)
        {
            bool result;
            string fileName = $"{FilePrefix}{linkDirectory.Id}.json";
            List<ITreeItemDTO> treeItemDTOs = _converter.GetTreeItemDTOs(new List<ITreeItem> { linkDirectory });
            try
            {
                File.WriteAllText(Path.Combine(FileSystem.Current.AppDataDirectory, fileName), JsonSerializer.Serialize(treeItemDTOs));
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public void SaveCollections(List<LinkDirectory> linkDirectories)
        {
            foreach (LinkDirectory directory in linkDirectories)
            {
                _ = SaveCollection(directory);
            }
        }

    }
}
