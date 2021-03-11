using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Handlers
{
    public interface IHttpHandler
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetStringAsync(string url);
    }
}
