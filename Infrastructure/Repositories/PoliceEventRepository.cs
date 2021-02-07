using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class PoliceEventRepository : IPoliceEventRepository
    {
        private readonly RepositorySettings _settings;
        private readonly ILogger<PoliceEventRepository> _logger;
        public PoliceEventRepository(IOptions<RepositorySettings> options, ILogger<PoliceEventRepository> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<PoliceEvent>> GetEventsForDate(DateTime fromDate,
                                                                     DateTime toDate,
                                                                     string locationName = null)
        {
            try
            {
                MongoClient dbClient = new MongoClient(_settings.ConnectionString);
                var database = dbClient.GetDatabase(_settings.PoliceDBName);
                var collection = database.GetCollection<BsonDocument>(_settings.PoliceEventCollectionName);
                var filter = Builders<BsonDocument>.Filter.Ne("Type", "Övrigt") &
                             Builders<BsonDocument>.Filter.Gte("UtcDateTime", new BsonDateTime(fromDate.Date)) &
                             Builders<BsonDocument>.Filter.Lt("UtcDateTime", new BsonDateTime(toDate.Date.AddDays(1)));
                if (locationName != null)
                {
                    filter &= Builders<BsonDocument>.Filter.Eq("Location.Name", locationName);
                }
                var asyncCursor = await collection.FindAsync<PoliceEvent>(filter);
                var events = asyncCursor.ToList<PoliceEvent>();
                return events;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw (e);
            }
        }

        public async Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents)
        {
            try
            {
                MongoClient dbClient = new MongoClient(_settings.ConnectionString);
                var database = dbClient.GetDatabase(_settings.PoliceDBName);
                var collection = database.GetCollection<BsonDocument>(_settings.PoliceEventCollectionName);
                var count = 0;
                foreach (var pe in policeEvents)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", pe.Id);
                    var replaceOptions = new ReplaceOptions
                    {
                        IsUpsert = true
                    };
                    await collection.ReplaceOneAsync(filter, pe.ToBsonDocument(), replaceOptions);
                    count++;
                }
                _logger.LogInformation($"Upserted {count} police events to the MongoDB database");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw (e);
            }
        }
    }
}
