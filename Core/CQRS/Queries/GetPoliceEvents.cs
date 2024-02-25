using Core.Helpers;
using Core.Models;
using Core.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Core.CQRS.Queries
{
    public abstract class GetPoliceEvents
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 5;
        public const int DefaultMaxDistKm = 1;
        public class QueryParameters
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string LocationName { get; set; }
            public double UserLat { get; set; }
            public double UserLng { get; set; }
            public double MaxDistanceKm { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public string FreeText { get; set; }
        }

        public class Query : IRequest<PoliceEventsResult>
        {
            public QueryParameters Parameters { get; }
            public Query(QueryParameters parameters) => Parameters = parameters;

        }

        public class Handler : IRequestHandler<Query, PoliceEventsResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IPoliceEventRepository _repository;
            public Handler(ILogger<Handler> logger, IPoliceEventRepository repository)
            {
                _logger = logger;
                _repository = repository;
            }

            public async Task<PoliceEventsResult> Handle(Query query, CancellationToken cancellationToken)
            {
                var defaultDate = DateTime.Now.Date;
                var from = DateHelper.GetParsedDateOrDefault(query.Parameters.FromDate, defaultDate);
                var to = DateHelper.GetParsedDateOrDefault(query.Parameters.ToDate, defaultDate);
                _logger.LogInformation(
                    "Get request: fromDate {FromDate}, toDate: {Date}, userLat: {UserLat}, userLng: {UserLng}",
                    from, to, query.Parameters.UserLat, query.Parameters.UserLng);

                var pagedResult = await _repository.GetEvents(cancellationToken,
                    from, to,
                    query.Parameters.Page == 0 ? DefaultPage : query.Parameters.Page,
                    query.Parameters.PageSize == 0 ? DefaultPageSize : query.Parameters.PageSize,
                    query.Parameters.UserLat,
                    query.Parameters.UserLng,
                    query.Parameters.MaxDistanceKm == 0 ? DefaultMaxDistKm : query.Parameters.MaxDistanceKm,
                    query.Parameters.LocationName,
                    query.Parameters.FreeText
                    );

                return pagedResult;
            }
        }
    }
}
