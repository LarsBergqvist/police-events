using Core.Clients;
using Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Core.CQRS.Commands
{
    public abstract class CollectPoliceEvents
    {
        public class Command : IRequest
        {
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IPoliceApiClient _policeApiClient;
            private readonly IPoliceEventRepository _policeEventRepository;
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger,
                           IPoliceApiClient policeApiClient,
                           IPoliceEventRepository policeEventRepository)
            {
                _logger = logger;
                _policeApiClient = policeApiClient;
                _policeEventRepository = policeEventRepository;
            }

            public async Task Handle(Command command, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Start fetching data");
                var events = await _policeApiClient.GetLatestEvents();

                _logger.LogInformation("Start upserting events in database");
                await _policeEventRepository.UpsertCollection(events);
                _logger.LogInformation("Finished upserting events in database");
            }
        }
    }
}

