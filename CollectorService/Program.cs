using Core.Clients;
using Core.Repositories;
using Core.Settings;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CollectorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddOptions()
                        .Configure<PoliceApiSettings>(hostContext.Configuration.GetSection("PoliceApiSettings"))
                        .Configure<RepositorySettings>(hostContext.Configuration.GetSection("RepositorySettings"))
                        .AddTransient<IPoliceApiClient, PoliceApiClient>()
                        .AddTransient<IPoliceEventRepository, PoliceEventRepository>()
                        .AddHostedService<Worker>()
                    ;
                });
    }
}
