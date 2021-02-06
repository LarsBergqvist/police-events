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
        public async Task<IEnumerable<PoliceEvent>> Get()
        {
            return await _repository.GetEventsForDate(DateTime.Now.Date);
        }
    }
}
