using Core.CQRS.Commands;
using Core.Settings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CollectorService
{
    public class Worker : IHostedService
    {
        private readonly PoliceApiSettings _settings;
        private readonly IMediator _mediator;
        private Timer _timer;

        public Worker(IOptions<PoliceApiSettings> options,
                      IMediator mediator)
        {
            _settings = options.Value;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CollectData, null, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(_settings.PollingIntervalMinutes));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private async void CollectData(object state)
        {
            await _mediator.Send(new CollectPoliceEvents.Command());
        }
    }
}
