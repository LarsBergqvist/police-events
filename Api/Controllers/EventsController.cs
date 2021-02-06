using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IPoliceEventRepository _repository;

        public EventsController(ILogger<EventsController> logger, IPoliceEventRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<PoliceEvent>> Get([FromQuery] string fromDate = "", [FromQuery] string toDate = "")
        {
            var from = DateTime.Now.Date;
            var to = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                if (DateTime.TryParseExact(fromDate,
                           "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out var parsedDate))
                {
                    from = parsedDate;
                }
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                if (DateTime.TryParseExact(toDate,
                           "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out var parsedDate))
                {
                    to = parsedDate;
                }
            }
            return await _repository.GetEventsForDate(from, to);
        }
    }
}
