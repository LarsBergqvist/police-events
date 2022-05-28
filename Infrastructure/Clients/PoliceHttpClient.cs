using Core.Clients;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Clients
{
    public class PoliceHttpClient : IPoliceHttpClient
    {
        private readonly HttpClient _client;

        public PoliceHttpClient(HttpClient httpClient)
        {
            _client = httpClient;
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
