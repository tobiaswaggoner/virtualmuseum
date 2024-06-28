namespace virtualmuseum.web.api.Services.Model;

public class Release
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long ApkAssetId { get; set; }
    public string? ApkAssetName { get; set; }
    public string? ApkAssetUrl { get; set; }
}