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
        public List<ITreeItem> GetTopLevelTreeItems()
        {
            List<ITreeItem> results = new();

            var files = Directory.EnumerateFiles(FileSystem.AppDataDirectory);
            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).StartsWith(FilePrefix))
                {
                    string jsonText = File.ReadAllText(file);
                    LinkDirectory directory = JsonSerializer.Deserialize<LinkDirectory>(jsonText);
                    results.Add(directory);
                }
            }

            return results;
        }

        public bool SaveCollection(LinkDirectory linkDirectory)
        {
            bool result;
            string fileName = $"{FilePrefix}{linkDirectory.Id}.json";
            try
            {
                File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, fileName), JsonSerializer.Serialize(linkDirectory));
                result = true;
            }
            catch (Exception)
            {
                result = false;
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
