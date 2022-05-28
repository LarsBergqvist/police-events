using Core.Helpers;
using Core.Models;
using Core.Repositories;
using Core.Settings;
using Infrastructure.MongoDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PoliceEventRepository : IPoliceEventRepository
    {
        private readonly RepositorySettings _settings;
        private readonly ILogger<PoliceEventRepository> _logger;
        private readonly IMongoDbContext _mongoDbContext;
        public PoliceEventRepository(IOptions<RepositorySettings> options, ILogger<PoliceEventRepository> logger,
                                     IMongoDbContext mongoDbContext)
        {
            _settings = options.Value;
            _logger = logger;
            _mongoDbContext = mongoDbContext;
        }

        public async Task<PoliceEvent> GetEventById(int id)
        {
            try
            {
                var collection = GetCollection<PoliceEventEntity>();
                var filter = Builders<PoliceEventEntity>.Filter.Eq("_id", id);
                var asyncCursor = await collection.FindAsync<PoliceEventEntity>(filter);
                var events = asyncCursor.ToList();
                if (events.Count == 0)
                {
                    return null;
                }
                events[0].Url = UrlHelper.CompleteEventUrl(events[0].Url);
                return events[0];
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }


        public async Task<PoliceEventsResult> GetEvents(DateTime fromDate,
            DateTime toDate,
            int page, int pageSize,
            double userLat, double userLng, double maxDistanceKm,
            string locationName = null,
            string freeText = null)
        {
            try
            {
                if (pageSize < 1) throw new ArgumentException("pageSize must be > 0");
                if (page < 1) throw new ArgumentException("page must be > 0");

                var collection = GetCollection<PoliceEventEntity>();

                var filter =
                    GetFilters<PoliceEventEntity>(fromDate, toDate, userLat, userLng, maxDistanceKm, locationName, freeText);

                var data = await collection.Find(filter)
                    .SortByDescending(x => x.UtcDateTime)
                    .Skip((page - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();

                //
                // Additional query for getting the total count
                // from a list as geoNear with count is not
                // supported in the same query
                //
                var simpleProjection = Builders<PoliceEventEntity>.Projection
                    .Include(u => u.Id);
                var count = (await collection.Find(filter).Project(simpleProjection).ToListAsync()).Count;

                var pagedResult = new PoliceEventsResult
                {
                    TotalPages = (int)Math.Ceiling((double)count / pageSize),
                    Page = page,
                    Events = data.Select(pe => pe.ToPoliceEvent()).ToList()
                };
                return pagedResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        private static FilterDefinition<TResponse> GetFilters<TResponse>(DateTime fromDate, DateTime toDate, double userLat, double userLng, double maxDistanceKm, string locationName, string freeText)
        {
            var filter = Builders<TResponse>.Filter.Ne("Type", "Övrigt") &
                         Builders<TResponse>.Filter.Gte("UtcDateTime", new BsonDateTime(fromDate.Date)) &
                         Builders<TResponse>.Filter.Lt("UtcDateTime", new BsonDateTime(toDate.Date.AddDays(1)));

            if (locationName != null && locationName.Length > 2)
            {
                filter &= Builders<TResponse>.Filter.Regex("Location.Name",
                    new BsonRegularExpression(".*" + locationName + ".*", "i"));
            }
            if (freeText != null && freeText.Length > 2)
            {
                filter &= Builders<TResponse>.Filter.Regex("Summary",
                    new BsonRegularExpression(".*" + freeText + ".*", "i"));
            }

            if (userLat > 0 && userLng > 0)
            {
                var gp = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(userLat, userLng));
                filter &= Builders<TResponse>.Filter.Near("Geo", gp, maxDistanceKm * 1000);
            }

            return filter;
        }

        public async Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents)
        {
            try
            {
                var collection = GetCollection<PoliceEventEntity>();
                var count = 0;
                foreach (var pe in policeEvents)
                {
                    var entity = new PoliceEventEntity(pe);
                    var filter = Builders<PoliceEventEntity>.Filter.Eq("_id", pe.Id);
                    var replaceOptions = new ReplaceOptions
                    {
                        IsUpsert = true
                    };
                    await collection.ReplaceOneAsync(filter, entity, replaceOptions);
                    count++;
                }
                _logger.LogInformation($"Upserted {count} police events to the MongoDB database");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        private IMongoCollection<TResponse> GetCollection<TResponse>()
        {
            return _mongoDbContext.GetCollection<TResponse>(_settings.PoliceEventCollectionName);
        }
    }
}
