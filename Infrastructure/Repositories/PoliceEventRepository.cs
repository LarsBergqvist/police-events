using System;
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
        private readonly RepositorySettings _settings;
        public PoliceEventRepository(IOptions<RepositorySettings> options)
        {
            _settings = options.Value;
        }

        public async Task<IEnumerable<PoliceEvent>> GetEventsForDate(DateTime fromDate, DateTime toDate)
        {
            MongoClient dbClient = new MongoClient(_settings.ConnectionString);
            var database = dbClient.GetDatabase(_settings.PoliceDBName);
            var collection = database.GetCollection<BsonDocument>(_settings.PoliceEventCollectionName);
            var filter = Builders<BsonDocument>.Filter.Gte("UtcDateTime", new BsonDateTime(fromDate.Date)) &
                         Builders<BsonDocument>.Filter.Lt("UtcDateTime", new BsonDateTime(toDate.Date.AddDays(1)));
            var asyncCursor = await collection.FindAsync<PoliceEvent>(filter);
            var events = asyncCursor.ToList<PoliceEvent>();
            return await Task.FromResult(events);
        }

        public async Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents)
        {
            MongoClient dbClient = new MongoClient(_settings.ConnectionString);
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
