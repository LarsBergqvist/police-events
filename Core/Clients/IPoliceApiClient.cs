using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Clients
{
    public interface IPoliceApiClient
    {
        Task<IEnumerable<Models.PoliceEvent>> GetLatestEvents();
    }
}
