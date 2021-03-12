using Core.Models;
using Core.Repositories;
using Core.Settings;
using Infrastructure.MongoDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PoliceEventRepository : IPoliceEventRepository
    {
        private readonly RepositorySettings _settings;
        private readonly ILogger<PoliceEventRepository> _logger;
        private readonly IMongoDBContext _mongoDBContext;
        public PoliceEventRepository(IOptions<RepositorySettings> options, ILogger<PoliceEventRepository> logger,
                                     IMongoDBContext mongoDBContext)
        {
            _settings = options.Value;
            _logger = logger;
            _mongoDBContext = mongoDBContext;
        }

        public async Task<PoliceEvent> GetEventById(int id)
        {
            try
            {
                var collection = GetCollection();
                var filter = Builders<PoliceEvent>.Filter.Eq("_id", id);
                var asyncCursor = await collection.FindAsync<PoliceEvent>(filter);
                var events = asyncCursor.ToList();
                if (events.Count == 0)
                {
                    return null;
                }
                return events[0];
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw (e);
            }
        }

        public async Task<IEnumerable<PoliceEvent>> GetEventsForDate(DateTime fromDate,
                                                                     DateTime toDate,
                                                                     string locationName = null)
        {
            try
            {
                var collection = GetCollection();
                var filter = Builders<PoliceEvent>.Filter.Ne("Type", "Övrigt") &
                             Builders<PoliceEvent>.Filter.Gte("UtcDateTime", new BsonDateTime(fromDate.Date)) &
                             Builders<PoliceEvent>.Filter.Lt("UtcDateTime", new BsonDateTime(toDate.Date.AddDays(1)));
                if (locationName != null)
                {
                    filter &= Builders<PoliceEvent>.Filter.Eq("Location.Name", locationName);
                }
                var asyncCursor = await collection.FindAsync<PoliceEvent>(filter);
                var events = asyncCursor.ToList();
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
                var collection = GetCollection();
                var count = 0;
                foreach (var pe in policeEvents)
                {
                    var filter = Builders<PoliceEvent>.Filter.Eq("_id", pe.Id);
                    var replaceOptions = new ReplaceOptions
                    {
                        IsUpsert = true
                    };
                    await collection.ReplaceOneAsync(filter, pe, replaceOptions);
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

        private IMongoCollection<PoliceEvent> GetCollection()
        {
            return _mongoDBContext.GetCollection<PoliceEvent>(_settings.PoliceEventCollectionName);
        }
    }
}
