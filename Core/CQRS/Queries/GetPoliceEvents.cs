using Core.Helpers;
using Core.Models;
using Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public class Query : IRequest<IEnumerable<PoliceEvent>>
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
                var defaultDate = DateTime.Now.Date;
                var from = DateHelper.GetParsedDateOrDefault(query.Parameters.FromDate, defaultDate);
                var to = DateHelper.GetParsedDateOrDefault(query.Parameters.ToDate, defaultDate);
                var coll = await _repository.GetEventsForDate(from, to, query.Parameters.LocationName);
                if (query.Parameters.UserLat == 0 || query.Parameters.UserLng == 0)
                {
                    return coll;
                }
                var filtered =
                    coll.Where(e => DistanceCalculator.GetDistanceKm(query.Parameters.UserLat, query.Parameters.UserLng,
                                                                     e.Location.Lat, e.Location.Lng) < query.Parameters.MaxDistanceKm);
                return filtered;
            }
        }
    }
}
