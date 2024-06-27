using System.Numerics;

namespace Server.Data
{


    /// <summary>
    /// An inventory placement defines where
    /// a piece of inventory is placed in the museum.
    /// how it is rotated and how it is scaled.
    /// </summary>
    public class InventoryPlacement
    {
        public InventoryItem? InventoryItem { get; set; }
        public Location? Location { get; set; }
    }
}