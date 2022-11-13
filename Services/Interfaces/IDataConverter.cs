using Pileolinks.Components.Tree;
using Pileolinks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Services.Interfaces
{
    public interface IDataConverter
    {
        List<ITreeItemDTO> GetTreeItemDTOs(List<ITreeItem> treeItems);
        List<ITreeItem> GetTreeItems(List<ITreeItemDTO> treeItems);
        
    }
}
