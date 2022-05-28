using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IPoliceEventRepository
    {
        Task UpsertCollection(IEnumerable<PoliceEvent> policeEvents);

        Task<PoliceEvent> GetEventById(int id);

        Task<PoliceEventsResult> GetEvents(
            DateTime fromDate,
            DateTime toDate,
            int page, int pageSize,
            double userLat, double userLng, double maxDistanceKm,
            string locationName = null,
            string freeText = null);
    }
}
