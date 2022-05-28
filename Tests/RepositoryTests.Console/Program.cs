﻿using Core.Extensions;
using Core.Repositories;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RepositoryTests.Console
{
    class Program
    {
        static async Task Main()
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
            var result = await repo.GetEvents(fromDate, toDate, 1, 5, 0, 0, 0);
            foreach (var pe in result.Events)
            {
                System.Console.WriteLine($"{pe.UtcDateTime}: {pe.Summary}");
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<Core.Settings.RepositorySettings>(configuration.GetSection("RepositorySettings"))
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton(configuration)
                .AddCoreServices()
                .AddInfrastructureServices()
                ;
        }
    }
}
