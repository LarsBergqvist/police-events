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
                var from = DateTime.Now.Date;
                var to = DateTime.Now.Date;
                if (!string.IsNullOrEmpty(request.Parameters.FromDate))
                {
                    if (DateTime.TryParseExact(request.Parameters.FromDate,
                               "yyyy-MM-dd",
                               System.Globalization.CultureInfo.InvariantCulture,
                               System.Globalization.DateTimeStyles.None,
                               out var parsedDate))
                    {
                        from = parsedDate;
                    }
                }
                if (!string.IsNullOrEmpty(request.Parameters.ToDate))
                {
                    if (DateTime.TryParseExact(request.Parameters.ToDate,
                               "yyyy-MM-dd",
                               System.Globalization.CultureInfo.InvariantCulture,
                               System.Globalization.DateTimeStyles.None,
                               out var parsedDate))
                    {
                        to = parsedDate;
                    }
                }
                return await _repository.GetEventsForDate(from, to, request.Parameters.LocationName);
            }
        }


    }
}
