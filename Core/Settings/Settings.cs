using System;
namespace Core.Settings
{
    public class Settings
    {
        public string PoliceApiUrl { get; set; }
        public string MongoDBConnectionString { get; set; }
        public string PoliceDBName { get; set; }
        public string PoliceEventCollectionName { get; set; }
    }
}
