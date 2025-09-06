using System.Text.Json.Serialization;

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
        string ItemType,
        HashSet<string> Tags,
        int Used = 0,
        DateOnly LastUsed = default
        ) : ITreeItemDTO;
}
