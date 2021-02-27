using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MediatR;
using Core.Commands;

namespace AzureFunctions
{
    public class CollectionTrigger
    {
        private readonly IMediator _mediator;


        public CollectionTrigger(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(CollectionTrigger))]
        public async Task Run(
            [TimerTrigger("0 */15 * * * *")]TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation($"CollectionTrigger triggered at:  {DateTime.Now}");
            await _mediator.Send(new UpsertPoliceEvents.Command());
        }
    }
}
