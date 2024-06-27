using System.Globalization;
using System.Text;
using virtualmuseum.web.data;

namespace virtualmuseum.web.api.Services;

public class ConfigurationRepository : IConfigurationRepository
{
    private readonly Guid _dummyRoomId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    private readonly Guid _dummyTableId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly List<Room> _rooms = [];
    private readonly Dictionary<Guid, TopographicalTableConfiguration> _tableConfigurations = new();

    public ConfigurationRepository()
    {
        InitializeRooms();
        InitializeTableConfigurations();
    }
    
    public List<Room> GetAllRooms() => _rooms.Select(r => new Room
    {
        Id = r.Id,
        Label = r.Label,
        Description = r.Description,
        InventoryPlacements = [],
    }).ToList();
    public Room GetRoom(Guid id) => _rooms.First(r => r.Id == id);
    public TopographicalTableConfiguration GetTopographicalTableConfiguration(Guid id) => _tableConfigurations[id];

    private void InitializeRooms()
    {
        var room1 = new Room
        {
            Id = _dummyRoomId,
            Label = "Der erste Raum",
            InventoryPlacements = GetInventoryPlacements(),
            Description = "Das ist die Beschreibung des Raums. Die ID sollte mit einem bestimmten Spatial Anchor verknüpft sein."
        };
        
        _rooms.Add(room1);
    }

    private List<InventoryPlacement> GetInventoryPlacements()
    {
        return [new InventoryPlacement
        {
            InventoryItem = new InventoryItem
            {
                Id = _dummyTableId,
                Label = "Topographischer Tisch",
                TypeOfItem = "TOPOGRAPHICAL_TABLE",
            },
            Location = new Location
            {
                PositionX = 100,
                PositionY = 100,
                PositionZ = 0,
            }
        }];
    }
    
    private void InitializeTableConfigurations()
    {
        var tableConfiguration = new TopographicalTableConfiguration
        {
            Id = _dummyTableId,
            Label = "Topographischer Tisch",
            LocationTimeRows = GetLocationTimeRows(),
        };
        
        _tableConfigurations.Add(tableConfiguration.Id, tableConfiguration);
    }

    private List<LocationTimeRow> GetLocationTimeRows()
    {
        var dummySections = new List<int>
        {
            704, 774, 785, 786, 799, 810, 839, 860, 874
        };
        var geoEvents = ReadCsv("InputData/MuseumGPS.csv");
        var locationTimeRows = new List<LocationTimeRow>();
        for(var i = 0; i < dummySections.Count-1; i++)
        {
            var yearStart = dummySections[i];
            var yearEnd = dummySections[i+1];
            var geoEventsInRange = geoEvents.Where(geoEvent => geoEvent.Year >= yearStart && geoEvent.Year < yearEnd).ToList();
            var locationTimeRow = new LocationTimeRow
            {
                Label = $"From {yearStart} to {yearEnd} ({geoEventsInRange.Count} events)",
                GeoEvents = geoEventsInRange,
            };
            locationTimeRows.Add(locationTimeRow);
        }

        return locationTimeRows;

    }

    private static List<GeoEvent> ReadCsv(string filePath)
    {
        var geoEvents = new List<GeoEvent>();

        var file = new StreamReader(filePath, Encoding.UTF8);
        while (!file.EndOfStream)
        {
            var line = file.ReadLine();
            if (line == null) continue;
            var parts = line.Split(';');
            var location = parts[3].Split(',');

            var geoEvent = new GeoEvent
            {
                Year = int.Parse(parts[0]),
                Label = parts[1],
                Latitude = double.Parse(location[0], CultureInfo.InvariantCulture),
                Longitude = double.Parse(location[1], CultureInfo.InvariantCulture),
                MediaFiles = [ new MediaFile { Id = Guid.Empty, Type = "JPG", Description = "Just a test", Name="Test Image", Url = $"/api/media/{Guid.Empty}/display"}]
            };

            geoEvents.Add(geoEvent);
        }

        return geoEvents;
    }

}