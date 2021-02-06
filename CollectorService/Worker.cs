using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Clients;
using Core.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CollectorService
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPoliceApiClient _policeApiClient;
        private readonly IPoliceEventRepository _policeEventRepository;
        private Timer _timer;

        public Worker(ILogger<Worker> logger, IPoliceApiClient policeApiClient, IPoliceEventRepository policeEventRepository)
        {
            _logger = logger;
            _policeApiClient = policeApiClient;
            _policeEventRepository = policeEventRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CollectData, null, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private async void CollectData(object state)
        {
            var events = await _policeApiClient.GetLatestEvents();
            await _policeEventRepository.UpsertCollection(events);
        }

    }
}
