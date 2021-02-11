using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IMediator _mediator;

        public EventsController(ILogger<EventsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<PoliceEvent>> Get([FromQuery] string fromDate = "", [FromQuery] string toDate = "")
        {
            _logger.LogInformation($"Get request: fromDate {fromDate}, toDate: {toDate}");
            var queryParams = new GetPoliceEvents.QueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            return await _mediator.Send(new GetPoliceEvents.Request(queryParams));
        }
    }
}
