using Core.Clients;
using Core.Models;
using Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Core.CQRS.Queries
{
    public abstract class GetPoliceEventById
    {
        public class Query : IRequest<PoliceEventDetails>
        {
            public int Id { get; }
            public Query(int id) => Id = id;

        }

        public class Handler : IRequestHandler<Query, PoliceEventDetails>
        {
            private readonly IPoliceEventRepository _repository;
            private readonly IPoliceApiClient _policeApiClient;
            private readonly ILogger<Handler> _logger;
            public Handler(IPoliceEventRepository repository, IPoliceApiClient policeApiClient, ILogger<Handler> logger)
            {
                _repository = repository;
                _policeApiClient = policeApiClient;
                _logger = logger;
            }

            public async Task<PoliceEventDetails> Handle(Query query, CancellationToken cancellationToken)
            {
                _logger.LogDebug("Fetching details for police event with id: {Id}", query.Id);
                var @event = await _repository.GetEventById(query.Id);
                var ext = new PoliceEventDetails
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    Location = @event.Location,
                    Summary = @event.Summary,
                    Type = @event.Type,
                    Url = @event.Url,
                    UtcDateTime = @event.UtcDateTime
                };

                var webScrapeResult = await _policeApiClient.WebScrapeDetails(@event.Url);
                ext.Details = webScrapeResult.Details;
                ext.Description = webScrapeResult.Description;

                return ext;
            }
        }
    }
}
