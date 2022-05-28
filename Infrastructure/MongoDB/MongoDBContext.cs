using Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.MongoDB
{
    public interface IMongoDbContext
    {
        IMongoCollection<TPoliceEventEntity> GetCollection<TPoliceEventEntity>(string name);
    }

    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IOptions<RepositorySettings> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            _db = mongoClient.GetDatabase(options.Value.PoliceDBName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}
