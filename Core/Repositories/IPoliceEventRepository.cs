using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Repositories
{
    public interface IPoliceEventRepository
    {
        Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents);
        Task<IEnumerable<PoliceEvent>> GetEventsForDate(DateTime fromDate, DateTime toDate);
    }
}
