using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<PoliceEvent>>>
            Get([FromQuery] string fromDate = "", [FromQuery] string toDate = "",
                [FromQuery] double userLat=0, [FromQuery] double userLng=0, [FromQuery] double maxKm = 0)
        {
            _logger.LogInformation($"Get request: fromDate {fromDate}, toDate: {toDate}, userLat: {userLat}, userLng: {userLng}");
            var queryParams = new GetPoliceEvents.QueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate,
                UserLat = userLat,
                UserLng = userLng,
                MaxDistanceKm = maxKm
            };
            return Ok(await _mediator.Send(new GetPoliceEvents.Query(queryParams)));
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PoliceEvent>> GetById(int id)
        {
            _logger.LogInformation($"Get request for id: {id}");
            var policeEvent = await _mediator.Send(new GetPoliceEventById.Query(id));
            if (policeEvent == null)
            {
                return NotFound();
            }
            return Ok(policeEvent);
        }
    }
}
