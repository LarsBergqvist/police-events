using Core.Handlers;
using Core.Models;
using Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.CQRS.Queries
{
    public class GetPoliceEventById
    {
        public class Query : IRequest<PoliceEventDetails>
        {
            public int Id { get; set; }
            public Query(int id) => Id = id;

        }

        public class Handler : IRequestHandler<Query, PoliceEventDetails>
        {
            private readonly IPoliceEventRepository _repository;
            private readonly IHttpHandler _httpHandler;
            private readonly ILogger<Handler> _logger;
            public Handler(IPoliceEventRepository repository, IHttpHandler httpHandler, ILogger<Handler> logger)
            {
                _repository = repository;
                _httpHandler = httpHandler;
                _logger = logger;
            }

            public async Task<PoliceEventDetails> Handle(Query query, CancellationToken cancellationToken)
            {
                var @event = await _repository.GetEventById(query.Id);
                var ext = new PoliceEventDetails
                {
                    Id = @event.Id,
                    Location = @event.Location,
                    Summary = @event.Summary,
                    Type = @event.Type,
                    Url = @event.Url,
                    UtcDateTime = @event.UtcDateTime
                };

                try
                {
                    //
                    // Web-scrape the referred html page and try to find additional details
                    //
                    var page = await _httpHandler.GetStringAsync(@event.Url);
                    var regex = new Regex(@"(?<=(<div class=""text-body editorial-html"">))(.|\n)*?(?=(<\/div>))", RegexOptions.Multiline);
                    var matches = regex.Match(page);
                    if (matches.Length > 0)
                    {
                        ext.Details = matches.Value;
                    }
                    regex = new Regex(@"(?<=(<p class=""preamble"">))(.|\n)*?(?=(<\/p>))", RegexOptions.Multiline);
                    matches = regex.Match(page);
                    if (matches.Length > 0)
                    {
                        ext.Description = matches.Value;
                    }

                }
                catch (Exception exc)
                {
                    _logger.LogError(exc.ToString());
                }

                return ext;
            }
        }
    }
}
