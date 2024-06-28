namespace virtualmuseum.web.data;

/// <summary>
/// A room defines one virtual setting within the museum.
/// It defines which items will be loaded into the room and where they are placed.
/// </summary>
public class Room
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public string ResourceUrl => "/api/rooms/" + Id;
    public List<MediaFile> MediaFiles { get; set; } = [];
    public List<InventoryPlacement> InventoryPlacements { get; set; } = [];
}