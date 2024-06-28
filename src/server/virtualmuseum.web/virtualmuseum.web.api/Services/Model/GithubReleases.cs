using Newtonsoft.Json;

namespace virtualmuseum.web.api.Services.Model;

public class GitHubRelease
{
    public string Url { get; set; }
    public string AssetsUrl { get; set; }
    public string UploadUrl { get; set; }
    public string HtmlUrl { get; set; }
    public long Id { get; set; }
    public Author Author { get; set; }
    public string NodeId { get; set; }
    public string TagName { get; set; }
    public string TargetCommitish { get; set; }
    public string Name { get; set; }
    public bool Draft { get; set; }
    public bool Prerelease { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public List<Asset> Assets { get; set; }
    public string TarballUrl { get; set; }
    public string ZipballUrl { get; set; }
    public string Body { get; set; }
}

public class Author
{
    public string Login { get; set; }
    public long Id { get; set; }
    public string NodeId { get; set; }
    public string AvatarUrl { get; set; }
    public string GravatarId { get; set; }
    public string Url { get; set; }
    public string HtmlUrl { get; set; }
    public string FollowersUrl { get; set; }
    public string FollowingUrl { get; set; }
    public string GistsUrl { get; set; }
    public string StarredUrl { get; set; }
    public string SubscriptionsUrl { get; set; }
    public string OrganizationsUrl { get; set; }
    public string ReposUrl { get; set; }
    public string EventsUrl { get; set; }
    public string ReceivedEventsUrl { get; set; }
    public string Type { get; set; }
    public bool SiteAdmin { get; set; }
}

public class Asset
{
    public string Url { get; set; }
    public long Id { get; set; }
    public string NodeId { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public Author Uploader { get; set; }
    [JsonProperty("content_type")]
    public string ContentType { get; set; }
    public string State { get; set; }
    public long Size { get; set; }
    public int DownloadCount { get; set; }
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
    public string BrowserDownloadUrl { get; set; }
}