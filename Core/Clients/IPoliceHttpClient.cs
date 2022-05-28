using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Clients
{
    public interface IPoliceHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetStringAsync(string url);
    }
}
