using Core.CQRS.Queries;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
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

        /// <summary>
        /// Fetches event data within a certain date interval and optionally within a particular radius from a position
        /// </summary>
        /// <param name="fromDate">Will use todays date if omitted</param>
        /// <param name="toDate">Will use todays date if omitted</param>
        /// <param name="userLat">Latitude center from where to calculate the radius (optional)</param>
        /// <param name="userLng">Longitude center from where to calculate the radius (optional)</param>
        /// <param name="maxKm">Include events within this radius from userLat/Lng</param>
        /// <param name="page">The result page to request (optional, default 1)</param>
        /// <param name="pageSize">The desired page size for the results (optional, default 5)</param>
        /// <param name="freeText">A query string to use for free text search summaries (optional, default null)</param>
        /// <param name="location">A query string to use for free text search locations(optional, default null)</param>
        /// <returns>A result object with paged results</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<PoliceEventsResult>>
            Get([FromQuery] string fromDate = "", [FromQuery] string toDate = "",
                [FromQuery] double userLat = 0, [FromQuery] double userLng = 0, [FromQuery] double maxKm = 0,
                [FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] string freeText = null,
                [FromQuery] string location = null
            )
        {
            _logger.LogInformation($"Get request: fromDate {fromDate}, toDate: {toDate}, userLat: {userLat}, userLng: {userLng}");
            var queryParams = new GetPoliceEvents.QueryParameters
            {
                FromDate = fromDate,
                ToDate = toDate,
                UserLat = userLat,
                UserLng = userLng,
                MaxDistanceKm = maxKm,
                Page = page,
                PageSize = pageSize,
                FreeText = freeText,
                LocationName = location
            };
            return Ok(await _mediator.Send(new GetPoliceEvents.Query(queryParams)));
        }

        /// <summary>
        /// Fetches the details of a specific police event
        /// </summary>
        /// <param name="id">The id of the police event to fetch details for</param>
        /// <returns>A detailed police event object</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PoliceEventDetails), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PoliceEventDetails>> GetById(int id)
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
