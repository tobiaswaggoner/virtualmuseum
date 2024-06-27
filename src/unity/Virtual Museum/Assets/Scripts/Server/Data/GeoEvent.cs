using System.Collections.Generic;

namespace Server.Data
{

    public class GeoEvent
    {
        public int Year { get; set; }
        public string? Label { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<MediaFile> MediaFiles { get; set; } = new ();
    }
}