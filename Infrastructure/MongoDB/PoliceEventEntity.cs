using Core.Helpers;
using Core.Models;
using System.Collections.Generic;

namespace Infrastructure.MongoDB
{
    public class Geo
    {
        public Geo(double lat, double lng)
        {
            type = "Point";
            coordinates = new List<double>
            {
                lng,
                lat
            };
        }
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class PoliceEventEntity : PoliceEvent
    {
        public PoliceEventEntity(PoliceEvent pe)
        {
            Id = pe.Id;
            UtcDateTime = pe.UtcDateTime;
            Summary = pe.Summary;
            Url = pe.Url;
            Type = pe.Type;
            Location = pe.Location;
            if (Location != null)
            {
                Geo = new Geo(Location.Lng, Location.Lat);
            }
        }

        public PoliceEvent ToPoliceEvent()
        {
            return new PoliceEvent
            {
                Id = Id,
                UtcDateTime = UtcDateTime,
                Summary = Summary,
                Url = UrlHelper.CompleteEventUrl(Url),
                Type = Type,
                Location = Location
            };
        }
        public Geo Geo { get; set; }
    }
}
