using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pileolinks.Models
{
    [JsonDerivedType(typeof(DirectoryDTO), typeDiscriminator:"directory")]
    [JsonDerivedType(typeof(LinkDTO), typeDiscriminator:"link")]
    public interface ITreeItemDTO
    {
        string ItemType { get; }
        string Id { get; }
    }
    
    public record DirectoryDTO(
        string Id,
        string Name,
        string AncestorId,
        List<string> DescendantIds,
        string ItemType
        ) : ITreeItemDTO;

    public record LinkDTO(
        string Id, 
        string Name, 
        string Description, 
        string Uri, 
        string AncestorId, 
        string ItemType
        ) : ITreeItemDTO;
}
