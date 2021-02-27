using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Core.Clients;
using Core.Repositories;

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
        public async void Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            log.LogInformation("Start fetching data");
            var events = await _policeApiClient.GetLatestEvents();
            log.LogInformation("Start upserting events in database");
            await _policeEventRepository.UpsertCollection(events);
            log.LogInformation("Finished upserting events in database");
        }
    }
}
