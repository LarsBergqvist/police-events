using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using MediatR;
using System.Linq;
using Core.Helpers;

namespace Core.CQRS.Queries
{
    public class GetPoliceEvents
    {
        public class QueryParameters
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string LocationName { get; set; }
            public double UserLat { get; set; }
            public double UserLng { get; set; }
            public double MaxDistanceKm { get; set; }
        }

        public class Query: IRequest<IEnumerable<PoliceEvent>>
        {
            public QueryParameters Parameters { get; set; }
            public Query(QueryParameters parameters) => Parameters = parameters;

        }

        public class Handler : IRequestHandler<Query, IEnumerable<PoliceEvent>>
        {
            private readonly IPoliceEventRepository _repository;
            public Handler(IPoliceEventRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<PoliceEvent>> Handle(Query query, CancellationToken cancellationToken)
            {
                var from = GetParsedDateOrDefault(query.Parameters.FromDate);
                var to = GetParsedDateOrDefault(query.Parameters.ToDate);
                var coll = await _repository.GetEventsForDate(from, to, query.Parameters.LocationName);
                if (query.Parameters.UserLat == 0 || query.Parameters.UserLng == 0 )
                {
                    return coll;
                }
                var filtered = coll.Where(e => DistanceCalculator.GetDistanceKm(query.Parameters.UserLat, query.Parameters.UserLng,
                                                                           e.Location.Lat, e.Location.Lng) < query.Parameters.MaxDistanceKm);
                return filtered;
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
