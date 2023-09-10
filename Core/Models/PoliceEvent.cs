using System;

namespace Core.Models
{
    public class Location
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class PoliceEvent
    {
        public int Id { get; set; }
        public DateTime UtcDateTime { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public Location Location { get; set; }
    }

    public class PoliceEventDetails : PoliceEvent
    {
        public string Description { get; set; }
        public string Details { get; set; }
    }

}
