using System;
using System.IO;
using System.Threading.Tasks;
using Core.Repositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RepositoryTests.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var repo = serviceProvider.GetService<IPoliceEventRepository>();

            var fromDate = new DateTime(2021, 02, 06);
            var toDate = new DateTime(2021, 02, 06);
            var res = await repo.GetEventsForDate(fromDate, toDate, "Uppsala");
            foreach(var pe in res)
            {
                System.Console.WriteLine($"{pe.UtcDateTime}: {pe.Summary}");
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<Core.Settings.RepositorySettings>(configuration.GetSection("RepositorySettings"))
                .AddLogging(configure => configure.AddConsole())
                .AddTransient<IPoliceEventRepository, PoliceEventRepository>()
                ;
        }
    }
}
