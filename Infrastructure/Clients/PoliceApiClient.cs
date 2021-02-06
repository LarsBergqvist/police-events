using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Clients;
using Core.Models;
using Core.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Clients
{
    public class PoliceApiClient : IPoliceApiClient
    {
        private readonly PoliceApiSettings _settings;
        public PoliceApiClient(IOptions<PoliceApiSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<IEnumerable<PoliceEvent>> GetLatestEvents()
        {
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
