using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pileolinks.Models
{
    internal interface TreeItemDTO
    {
        string ItemType { get; }
        string Id { get; }
    }
    
    internal record DirectoryDTO(
        string Id,
        string Name,
        string AncestorId,
        List<string> DescendantIds,
        string ItemType
        ) : TreeItemDTO;

    internal record LinkDTO(
        string Id, 
        string Name, 
        string Description, 
        string Uri, 
        string AncestorId, 
        string ItemType
        ) : TreeItemDTO;
}
