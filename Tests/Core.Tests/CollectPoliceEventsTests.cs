using Core.Clients;
using Core.CQRS.Commands;
using Core.Models;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests
{
    public class CollectPoliceEventsTests
    {
        [Fact]
        public async Task RepositoryShouldBeUpsertedWithFetchedEvents()
        {
            //
            // Arrange
            //
            var events = new List<PoliceEvent>
            { new PoliceEvent{ Id=123, Summary= "summary123" },
              new PoliceEvent{ Id=456, Summary= "summary456" }
            };
            var clientMock = new Mock<IPoliceApiClient>();
            clientMock.SetReturnsDefault(Task.FromResult(events.AsEnumerable()));
            var repositoryMock = new Mock<IPoliceEventRepository>();
            var loggerMock = new Mock<ILogger<CollectPoliceEvents.Handler>>();

            var command = new CollectPoliceEvents.Command();
            var handler = new CollectPoliceEvents.Handler(loggerMock.Object, clientMock.Object, repositoryMock.Object);

            //
            // Act
            //
            await handler.Handle(command, new System.Threading.CancellationToken());

            //
            // Assert
            //
            clientMock.Verify(a => a.GetLatestEvents(), Times.Once);
            repositoryMock.Verify(a => a.UpsertCollection(events), Times.Once);
        }
    }
}
