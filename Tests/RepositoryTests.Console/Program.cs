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
            var settings = new Core.Settings.RepositorySettings
            {
                ConnectionString = "mongodb://192.168.1.186:27017",
                PoliceDBName = "Police",
                PoliceEventCollectionName = "police_events"
            };
            var options = Options.Create<Core.Settings.RepositorySettings>(settings);
            var repo = new PoliceEventRepository(options);

            var fromDate = new DateTime(2021, 02, 06);
            var toDate = new DateTime(2021, 02, 06);
            var res = await repo.GetEventsForDate(fromDate, toDate);
            foreach(var pe in res)
            {
                System.Console.WriteLine($"{pe.UtcDateTime}: {pe.Summary}");
            }
        }
    }
}
