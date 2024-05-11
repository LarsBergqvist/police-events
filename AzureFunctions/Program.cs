using Core.DI;
using Infrastructure.DI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddCoreServices();
        s.AddInfrastructureServices();
    })
    .ConfigureLogging(logging =>
    {
//        logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
    })
    .Build();

host.Run();