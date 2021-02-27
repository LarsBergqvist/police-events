using System.Threading;
using System.Threading.Tasks;
using Core.Clients;
using Core.Repositories;
using Core.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Commands
{
    public class UpsertPoliceEvents
    {
        public class Command : IRequest
        {
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IPoliceApiClient _policeApiClient;
            private readonly IPoliceEventRepository _policeEventRepository;
            private readonly PoliceApiSettings _settings;
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger,
                           IOptions<PoliceApiSettings> options,
                           IPoliceApiClient policeApiClient,
                           IPoliceEventRepository policeEventRepository)
            {
                _logger = logger;
                _settings = options.Value;
                _policeApiClient = policeApiClient;
                _policeEventRepository = policeEventRepository;
            }

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Start fetching data");
                var events = await _policeApiClient.GetLatestEvents();

                _logger.LogInformation("Start upserting events in database");
                await _policeEventRepository.UpsertCollection(events);
                _logger.LogInformation("Finished upserting events in database");

                return new MediatR.Unit();
            }
        }
    }
}

   