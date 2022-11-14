using Core.CQRS.Commands;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions
{
    public class CollectionTrigger
    {
        private readonly ISender _mediator;


        public CollectionTrigger(ISender mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(CollectionTrigger))]
        public async Task Run(
            [TimerTrigger("0 */15 * * * *")] TimerInfo timerInfo,
            ILogger log)
        {
            log.LogInformation("CollectionTrigger triggered at:  {Now}", DateTime.Now);
            await _mediator.Send(new CollectPoliceEvents.Command());
        }
    }
}
