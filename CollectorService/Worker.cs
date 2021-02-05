using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CollectorService
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EventFetcher _eventFetcher;
        private Timer _timer;

        public Worker(ILogger<Worker> logger, EventFetcher eventFetcher)
        {
            _logger = logger;
            _eventFetcher = eventFetcher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CollectData, null, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private async void CollectData(object state)
        {
            var events = await _eventFetcher.Fetch();
            MongoClient dbClient = new MongoClient("mongodb://192.168.1.186:27017");

            var database = dbClient.GetDatabase("Police");
            var collection = database.GetCollection<BsonDocument>("police_events");
            foreach (var pe in events)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", pe.Id);
                var replaceOptions = new ReplaceOptions();
                replaceOptions.IsUpsert = true;
                collection.ReplaceOne(filter, pe.ToBsonDocument(), replaceOptions);
            }
        }

    }
}
