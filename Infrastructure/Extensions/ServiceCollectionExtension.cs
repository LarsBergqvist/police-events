using Core.Clients;
using Core.Repositories;
using Core.Settings;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddOptions<PoliceApiSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("PoliceApiSettings").Bind(settings);
            });
            services.AddOptions<RepositorySettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("RepositorySettings").Bind(settings);
            });
            return services
                        .AddTransient<IPoliceApiClient, PoliceApiClient>()
                        .AddTransient<IPoliceEventRepository, PoliceEventRepository>();
            ;
        }
    }
}
