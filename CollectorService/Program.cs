using Core.Extensions;
using Core.Settings;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CollectorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetCurrentDirectory();
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
                        .AddCoreServices()
                        .AddInfrastructureServices()
                        .AddHostedService<Worker>()
                    ;
                });

        private static void SetCurrentDirectory()
        {
            var assemblyLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.IO.Directory.SetCurrentDirectory(assemblyLocation);
        }

    }
}
