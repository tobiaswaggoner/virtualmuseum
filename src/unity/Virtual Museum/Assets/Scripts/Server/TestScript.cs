using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server;
using Server.Data;
using UnityEngine;

[RequireComponent(typeof(ConfigurationManager))]
public class TestScript : MonoBehaviour
{
    List<Room> rooms;
    Room firstRoom;
    TopographicalTableConfiguration tableConfiguration;
    InventoryPlacement tablePlacement;
    
    private ConfigurationManager configurationManager;
    // Start is called before the first frame update
    void Start()
    {
        configurationManager = GetComponent<ConfigurationManager>();
        LoadConfiguration()
            .ContinueWith( task=> Debug.LogError(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
    }
    
    async Task LoadConfiguration()
    {
        rooms = await configurationManager.ConfigurationClient.GetRooms();
        Debug.Log("Rooms:");
        Debug.Log(JsonConvert.SerializeObject(rooms, Formatting.Indented));
        if(rooms.Count==0)
            return;
        firstRoom = await configurationManager.ConfigurationClient.GetRoom(rooms[0].Id);
        Debug.Log($"First Room: {firstRoom.Label} ");
        
        tablePlacement = firstRoom.InventoryPlacements.FirstOrDefault( p=>p.InventoryItem?.TypeOfItem=="TOPOGRAPHICAL_TABLE");
        Debug.Log($"Table Placement: {tablePlacement?.InventoryItem?.Label}");
        if(tablePlacement==null)
            return;
        tableConfiguration = await configurationManager.ConfigurationClient.GetTableConfiguration(tablePlacement.InventoryItem!.Id);
        Debug.Log($"Table Configuration: {tableConfiguration.Label} {tableConfiguration.LocationTimeRows.Count} rows");
    }

}