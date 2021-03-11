using System.Net.Http;
using System.Threading.Tasks;
using Core.Handlers;

namespace Infrastructure.Handlers
{
    public class HttpHandler: IHttpHandler
    {
        private readonly HttpClient _client;

        public HttpHandler(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "police-events-app");
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await _client.GetStringAsync(url);
        }
    }
}
