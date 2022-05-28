using Core.Clients;
using Core.Models;
using Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Clients
{
    public class PoliceApiClient : IPoliceApiClient
    {
        private readonly PoliceApiSettings _settings;
        private readonly ILogger<PoliceApiClient> _logger;
        private readonly IPoliceHttpClient _policeHttpClient; 
        public PoliceApiClient(IOptions<PoliceApiSettings> options, ILogger<PoliceApiClient> logger,
                               IPoliceHttpClient policeHttpClient)
        {
            _settings = options.Value;
            _logger = logger;
            _policeHttpClient = policeHttpClient;
        }

        public async Task<IEnumerable<PoliceEvent>> GetLatestEvents()
        {
            _logger.LogInformation($"Get events from ApiUrl: {_settings.PoliceApiUrl}");
            var policeEventCollection = new List<PoliceEvent>();
            var res = await _policeHttpClient.GetAsync(_settings.PoliceApiUrl);
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

        public async Task<WebScrapeResult> WebScrapeDetails(string url)
        {
            var result = new WebScrapeResult();
            try
            {
                //
                // Web-scrape the referred html page and try to find additional details
                //
                var page = await _policeHttpClient.GetStringAsync(url);
                var regex = new Regex(@"(?<=(<div class=""text-body editorial-html"">))(.|\n)*?(?=(<\/div>))", RegexOptions.Multiline);
                var matches = regex.Match(page);
                if (matches.Length > 0)
                {
                    result.Details = matches.Value;
                }
                regex = new Regex(@"(?<=(<p class=""preamble"">))(.|\n)*?(?=(<\/p>))", RegexOptions.Multiline);
                matches = regex.Match(page);
                if (matches.Length > 0)
                {
                    result.Description = matches.Value;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
            }

            return result;
        }
    }
}
