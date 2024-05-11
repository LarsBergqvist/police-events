using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DI
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            return services
                .AddMediatR(config =>
                {
                    config.RegisterServicesFromAssembly(typeof(AssemblyClass).Assembly);
                });
        }
    }
}
