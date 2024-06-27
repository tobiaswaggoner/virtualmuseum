using virtualmuseum.web.data;

namespace virtualmuseum.web.api.Services;

public interface IConfigurationRepository
{
    List<Room> GetAllRooms();
    Room GetRoom(Guid id);
    TopographicalTableConfiguration GetTopographicalTableConfiguration(Guid id);
}