using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Clients;
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
        public PoliceApiClient(IOptions<PoliceApiSettings> options, ILogger<PoliceApiClient> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<IEnumerable<PoliceEvent>> GetLatestEvents()
        {
            _logger.LogInformation($"Get events from ApiUrl: {_settings.PoliceApiUrl}");
            var policeEventCollection = new List<PoliceEvent>();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "police-event-app");
            var res = await client.GetAsync(_settings.PoliceApiUrl);
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
