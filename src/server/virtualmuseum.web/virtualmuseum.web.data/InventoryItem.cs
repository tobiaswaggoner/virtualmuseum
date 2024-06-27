namespace virtualmuseum.web.data;

/// <summary>
/// An Inventory Item represents any virtual object in the
/// environment of the museum as defined by its type
/// The configuration data can be obtained using the Id
/// of the Item.
/// </summary>
public class InventoryItem
{
    public Guid  Id { get; set; }
    public string? Label { get; set; }
    public string? TypeOfItem { get; set; }
    public string ResourceUrl => TypeOfItem switch
    {
        "TOPOGRAPHICAL_TABLE" => "/api/topographical-table/" + Id,
        _ => ""
    };

}