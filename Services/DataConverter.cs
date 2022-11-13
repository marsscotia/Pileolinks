using Pileolinks.Components.Tree;
using Pileolinks.Models;
using Pileolinks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Services
{
    internal class DataConverter : IDataConverter
    {
        public List<TreeItemDTO> GetTreeItemDTOs(List<ITreeItem> treeItems)
        {
            List<TreeItemDTO> treeDTOs = new();

            Stack<ITreeItem> stack = new();
            foreach (ITreeItem item in treeItems)
            {
                stack.Push(item);
            }

            while (stack.Count > 0)
            {
                ITreeItem current = stack.Pop();
                switch (current.Type)
                {
                    case TreeItemType.Directory:
                        treeDTOs.Add(new DirectoryDTO
                        (
                            Id : current.Id,
                            Name : current.Name,
                            ItemType : current.Type.ToString(),
                            DescendantIds : current.Descendants.Select(d => d.Id).ToList(),
                            AncestorId : current.Ancestor?.Id
                        ));
                        foreach (ITreeItem child in current.Descendants)
                        {
                            stack.Push(child);
                        }
                        break;
                    case TreeItemType.Link:
                        treeDTOs.Add(new LinkDTO(
                            Id: current.Id,
                            Name: current.Name,
                            ItemType: current.Type.ToString(),
                            AncestorId: current.Ancestor.Id,
                            Description : ((Link)current).Description,
                            Uri : ((Link)current).Uri));
                        break;
                    default:
                        break;
                }
            }

            return treeDTOs;
        }

        public List<ITreeItem> GetTreeItems(List<TreeItemDTO> treeItems)
        {
            List<ITreeItem> results = new();

            List<ITreeItem> items = new();

            //the first time round the collection, we inflate the tree items but do not set ancestors or descendants
            foreach (var item in treeItems)
            {
                bool success = Enum.TryParse<TreeItemType>(item.ItemType, out TreeItemType treeItemType);
                if (success) 
                {
                    switch (treeItemType)
                    {
                        case TreeItemType.Directory:
                            DirectoryDTO directoryDTO = (DirectoryDTO)item;
                            items.Add(new LinkDirectory(directoryDTO.Id, null, directoryDTO.Name, new()));
                            break;
                        case TreeItemType.Link:
                            LinkDTO linkDTO = (LinkDTO)item;
                            items.Add(new Link(linkDTO.Id, null, linkDTO.Uri, linkDTO.Name)
                            {
                                Description = linkDTO.Description
                            });
                            break;
                        default:
                            break;
                    }
                }
            }

            //the second time round, we fetch the correct inflated item and set ancestors and descendants
            foreach (var item in treeItems)
            {
                ITreeItem treeItem = items.First(t => t.Id == item.Id);
                switch (treeItem.Type)
                {
                    case TreeItemType.Directory:
                        LinkDirectory directory = (LinkDirectory)treeItem;
                        DirectoryDTO directoryDTO = (DirectoryDTO)item;
                        if (directoryDTO.AncestorId is not null)
                        {
                            directory.MoveToDirectory(items.First(t => t.Id == directoryDTO.AncestorId));
                        }
                        break;
                    case TreeItemType.Link:
                        Link link = (Link)treeItem;
                        LinkDTO linkDTO = (LinkDTO)item;
                        link.MoveToDirectory(items.First(t => t.Id == linkDTO.AncestorId));
                        break;
                    default:
                        break;
                }
            }

            results.AddRange(items.Where(t => t.Ancestor is null));

            return results;
        }
    }
}
