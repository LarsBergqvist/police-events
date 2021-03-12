using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            return services
                      .AddMediatR(typeof(Core.AssemblyClass))
                  ;
        }
    }
}
