using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IPoliceEventRepository
    {
        Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents);
        Task<IEnumerable<PoliceEvent>> GetEventsForDate(DateTime fromDate, DateTime toDate, string locationName);
        Task<PoliceEvent> GetEventById(int id);
    }
}
