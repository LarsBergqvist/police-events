using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Clients
{
    public class WebScrapeResult
    {
        public string Details { get; set; }
        public string Description { get; set; }
    }

    public interface IPoliceApiClient
    {
        Task<IEnumerable<Models.PoliceEvent>> GetLatestEvents();
        Task<WebScrapeResult> WebScrapeDetails(string url);
    }
}
