using Core.Clients;
using Core.Repositories;
using Core.Settings;
using Infrastructure.Clients;
using Infrastructure.MongoDB;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddOptions<RepositorySettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("RepositorySettings").Bind(settings);
            });

            services.AddOptions<PoliceApiSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("PoliceApiSettings").Bind(settings);
            });
            services.AddOptions<RepositorySettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("RepositorySettings").Bind(settings);
            });
            services.AddHttpClient<IPoliceHttpClient, PoliceHttpClient>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "police-events-app");
            });
            return services
                        .AddTransient<IMongoDbContext, MongoDbContext>()
                        .AddTransient<IPoliceApiClient, PoliceApiClient>()
                        .AddTransient<IPoliceEventRepository, PoliceEventRepository>()
                        ;
        }
    }
}
