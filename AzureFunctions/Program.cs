using Core.DI;
using Infrastructure.DI;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddCoreServices();
        s.AddInfrastructureServices();
    })
    .Build();

host.Run();