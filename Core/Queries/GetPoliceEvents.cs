using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using MediatR;

namespace Core.Queries
{
    public class GetPoliceEvents
    {
        public class QueryParameters
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string LocationName { get; set; }
        }

        public class Request: IRequest<IEnumerable<PoliceEvent>>
        {
            public QueryParameters Parameters { get; set; }
            public Request(QueryParameters parameters) => Parameters = parameters;

        }

        public class Handler : IRequestHandler<Request, IEnumerable<PoliceEvent>>
        {
            private readonly IPoliceEventRepository _repository;
            public Handler(IPoliceEventRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<PoliceEvent>> Handle(Request request, CancellationToken cancellationToken)
            {
                var from = GetParsedDateOrDefault(request.Parameters.FromDate);
                var to = GetParsedDateOrDefault(request.Parameters.ToDate);
                return await _repository.GetEventsForDate(from, to, request.Parameters.LocationName);
            }

            private DateTime GetParsedDateOrDefault(string dateString)
            {
                var date = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(dateString))
                {
                    if (DateTime.TryParseExact(dateString,
                               "yyyy-MM-dd",
                               System.Globalization.CultureInfo.InvariantCulture,
                               System.Globalization.DateTimeStyles.None,
                               out var parsedDate))
                    {
                        date = parsedDate;
                    }
                }
                return date;
            }
        }
    }
}
