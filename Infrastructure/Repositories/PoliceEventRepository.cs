using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class PoliceEventRepository : IPoliceEventRepository
    {
        private readonly Settings _settings;
        public PoliceEventRepository(IOptions<Settings> options)
        {
            _settings = options.Value;
        }

        public async Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents)
        {
            MongoClient dbClient = new MongoClient(_settings.MongoDBConnectionString);
            var database = dbClient.GetDatabase(_settings.PoliceDBName);
            var collection = database.GetCollection<BsonDocument>(_settings.PoliceEventCollectionName);
            foreach (var pe in policeEvents)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", pe.Id);
                var replaceOptions = new ReplaceOptions
                {
                    IsUpsert = true
                };
                await collection.ReplaceOneAsync(filter, pe.ToBsonDocument(), replaceOptions);
            }
        }
    }
}
