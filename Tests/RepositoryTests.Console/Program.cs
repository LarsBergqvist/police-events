using System;
using System.Threading.Tasks;
using Infrastructure.Repositories;
using Microsoft.Extensions.Options;
namespace RepositoryTests.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new Core.Settings.Settings
            {
                MongoDBConnectionString = "mongodb://192.168.1.186:27017",
                PoliceDBName = "Police",
                PoliceEventCollectionName = "police_events"
            };
            var options = Options.Create<Core.Settings.Settings>(settings);
            var repo = new PoliceEventRepository(options);
            var res = await repo.GetEventsForDate(DateTime.Now.Date);
            foreach(var pe in res)
            {
                System.Console.WriteLine($"{pe.UtcDateTime}: {pe.Summary}");
            }
        }
    }
}
