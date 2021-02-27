using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Core.Clients;
using Core.Repositories;
using System.Threading.Tasks;

namespace AzureFunctions
{
    public class CollectionTrigger
    {
        private readonly IPoliceApiClient _policeApiClient;
        private readonly IPoliceEventRepository _policeEventRepository;

        public CollectionTrigger(IPoliceApiClient policeApiClient, IPoliceEventRepository policeEventRepository)
        {
            _policeApiClient = policeApiClient;
            _policeEventRepository = policeEventRepository;
        }

        [FunctionName(nameof(CollectionTrigger))]
        public async Task Run(
            [TimerTrigger("0 */15 * * * *")]TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation($"Start fetching data at:  {DateTime.Now}");
            var events = await _policeApiClient.GetLatestEvents();
            log.LogInformation("Start upserting events in database");
            await _policeEventRepository.UpsertCollection(events);
            log.LogInformation("Finished upserting events in database");
        }
    }
}
