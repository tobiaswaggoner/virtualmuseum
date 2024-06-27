using Microsoft.AspNetCore.Mvc;
using virtualmuseum.web.api.Services;
using virtualmuseum.web.data;

namespace virtualmuseum.web.api.Api;

[ApiController]
[Route("api")]
public class ConfigurationController : Controller
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(IConfigurationRepository configurationRepository, ILogger<ConfigurationController> logger)
    {
        _configurationRepository = configurationRepository;
        _logger = logger;
    }
    
    [HttpGet("rooms")]
    public IEnumerable<Room> Get()
    {
        try
        {
            return _configurationRepository.GetAllRooms();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting rooms");
            return [];
        }
    }
    
    [HttpGet("rooms/{id}")]
    public Room Get(Guid id)
    {
        try
        {
            return _configurationRepository.GetRoom(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting room");
            return new Room();
        }
    }
    
    [HttpGet("topographical-table/{id}")]
    public TopographicalTableConfiguration GetTopographicalTableConfiguration(Guid id)
    {
        try
        {
            return _configurationRepository.GetTopographicalTableConfiguration(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting topographical table configuration");
            return new TopographicalTableConfiguration();
        }
    }
}
