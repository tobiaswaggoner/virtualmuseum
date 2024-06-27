namespace virtualmuseum.web.data;

/// <summary>
/// A location time row represents a list of events
/// that happened at a specific location at a specific time.
/// </summary>
public class LocationTimeRow
{
    public string? Label { get; set; }
    public List<GeoEvent> GeoEvents { get; set; } = [];
    public List<MediaFile> MediaFiles { get; set; } = [];
}