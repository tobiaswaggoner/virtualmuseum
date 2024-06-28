using virtualmuseum.web.api.Services.Model;

namespace virtualmuseum.web.api.Services;

public interface IReleaseService
{
    Task<List<Release>> GetAllReleases();
    Task<List<Release>> Refresh();
}