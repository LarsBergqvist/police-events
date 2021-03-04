using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Clients;
using Core.Handlers;
using Core.Models;
using Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Clients
{
    public class PoliceApiClient : IPoliceApiClient
    {
        private readonly PoliceApiSettings _settings;
        private readonly ILogger<PoliceApiClient> _logger;
        private readonly IHttpHandler _httpHandler;
        public PoliceApiClient(IOptions<PoliceApiSettings> options, ILogger<PoliceApiClient> logger,
                               IHttpHandler httpHandler)
        {
            _settings = options.Value;
            _logger = logger;
            _httpHandler = httpHandler;
        }

        public async Task<IEnumerable<PoliceEvent>> GetLatestEvents()
        {
            _logger.LogInformation($"Get events from ApiUrl: {_settings.PoliceApiUrl}");
            var policeEventCollection = new List<PoliceEvent>();
            var res = await _httpHandler.GetAsync(_settings.PoliceApiUrl);
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var externalData = JsonConvert.DeserializeObject<PoliceEventExternal[]>(result);
                foreach (var ext in externalData)
                {
                    var policeEvent = ext.GetPoliceEvent();
                    policeEventCollection.Add(policeEvent);
                }
            }
            return policeEventCollection;
        }
    }
}
