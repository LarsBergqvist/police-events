using Core.Models;
using System;
using System.Globalization;

namespace Infrastructure.Clients
{
    public class LocationExternal
    {
        public string Name { get; set; }
        public string Gps { get; set; }
    }

    /// <summary>
    /// This is the model returned from the external api
    /// </summary>
    public class PoliceEventExternal
    {
        static readonly CultureInfo Provider = new CultureInfo("en-US");
        public int Id { get; set; }
        public string Name { get; set; }
        public string Datetime { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public LocationExternal Location { get; set; }

        /// <summary>
        /// Converts the external model to an internal PoliceEvent-model
        /// </summary>
        /// <returns>A PoliceEvent object</returns>
        public PoliceEvent GetPoliceEvent()
        {
            var e = new PoliceEvent
            {
                Id = Id,
                Name = Name,
                UtcDateTime = DateTime.Parse(Datetime).ToUniversalTime(),
                Summary = Summary,
                Url = Url,
                Type = Type
            };
            var parts = Location.Gps.Split(",");
            e.Location = new Location
            {
                Name = Location.Name,
                Lat = double.Parse(parts[0], Provider),
                Lng = double.Parse(parts[1], Provider)
            };
            return e;
        }
    }

}
