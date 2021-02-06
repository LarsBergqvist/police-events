using System;
using Core.Models;

namespace Infrastructure.Clients
{
    public class LocationExternal
    {
        public string Name { get; set; }
        public string Gps { get; set; }
    }
    public class PoliceEventExternal
    {
        public int Id { get; set; }
        public string Datetime { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public LocationExternal Location { get; set; }

        public PoliceEvent GetPoliceEvent()
        {
            var e = new PoliceEvent();
            e.Id = Id;
            e.UtcDateTime = DateTime.Parse(Datetime).ToUniversalTime();
            e.Summary = Summary;
            e.Url = Url;
            e.Type = Type;
            var parts = Location.Gps.Split(",");
            e.Location = new Location
            {
                Name = Location.Name,
                Lat = double.Parse(parts[0]),
                Lng = double.Parse(parts[1])
            };
            return e;
        }
    }

}
