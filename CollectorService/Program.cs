using Core.DI;
using Infrastructure.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CollectorService;

public class Program
{
    public static void Main(string[] args)
    {
        SetCurrentDirectory();
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services
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