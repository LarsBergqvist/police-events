using Core.CQRS.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
namespace AzureFunctions
{
    public class CollectionTrigger
    {
        private readonly ISender _mediator;
        private readonly Logger<CollectionTrigger> _logger;


        public CollectionTrigger(ISender mediator, Logger<CollectionTrigger> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Function(nameof(CollectionTrigger))]
        public async Task Run(
            [TimerTrigger("*/15 * * * * *")] TimerInfo timerInfo)
        {
            _logger.LogInformation("CollectionTrigger triggered at:  {Now}", DateTime.Now);
            await _mediator.Send(new CollectPoliceEvents.Command());
        }
    }
}
