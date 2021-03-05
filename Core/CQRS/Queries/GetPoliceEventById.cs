using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using MediatR;

namespace Core.CQRS.Queries
{
    public class GetPoliceEventById
    {
        public class Query: IRequest<PoliceEvent>
        {
            public int Id { get; set; }
            public Query(int id) => Id = id;

        }

        public class Handler : IRequestHandler<Query, PoliceEvent>
        {
            private readonly IPoliceEventRepository _repository;
            public Handler(IPoliceEventRepository repository)
            {
                _repository = repository;
            }

            public async Task<PoliceEvent> Handle(Query query, CancellationToken cancellationToken)
            {
                return await _repository.GetEventById(query.Id);
            }
        }
    }
}
