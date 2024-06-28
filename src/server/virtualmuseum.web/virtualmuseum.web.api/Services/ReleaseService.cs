using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using virtualmuseum.web.api.Services.Configuration;
using virtualmuseum.web.api.Services.Model;

namespace virtualmuseum.web.api.Services;

public class ReleaseService : IReleaseService
{
    private readonly ReleaseServiceConfig _configuration;
    private readonly HttpClient _httpClient;
    private List<Release> _releases = [];

    public ReleaseService(IOptions<ReleaseServiceConfig> configuration, HttpClient httpClient)
    {
        _configuration = configuration.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration.GithubBaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "VirtualMuseumInstaller");
        _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        if(_configuration.GithubPersonalAccessToken is not null)
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration.GithubPersonalAccessToken}" );
    }

    public async Task<List<Release>> Refresh()
    {
        _releases = [];
        return await GetAllReleases();
    }

    public async Task<List<Release>> GetAllReleases()
    {
        if(_releases.Any())
            return _releases;
        
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        var response = await _httpClient.GetAsync("");
        if (!response.IsSuccessStatusCode) return [];

        var content = response.Content.ReadAsStringAsync().Result;

        var gitHubReleases = JsonConvert
            .DeserializeObject<List<GitHubRelease>>(content, settings);
        
        if (gitHubReleases is null)
            return [];

        var releases = gitHubReleases
            .Select(ToRelease)
            .Where(r => r is not null)
            .Cast<Release>()
            .OrderByDescending(r=>r.PublishedAt)
            .ToList();
        _releases = releases;
        return releases;
    }

    private Release? ToRelease(GitHubRelease ghRelease)
    {
        var asset = ghRelease.Assets.FirstOrDefault(a => a.ContentType == "application/vnd.android.package-archive");
        if (asset is null)
            return null;
        
        return new Release
        {
            Id = ghRelease.Id,
            Name = ghRelease.Name,
            PublishedAt = ghRelease.PublishedAt,
            CreatedAt = ghRelease.CreatedAt,
            ApkAssetId = asset.Id,
            ApkAssetName = asset.Name,
            ApkAssetUrl = asset.BrowserDownloadUrl
        }; 
        
    }
}