using Core.Models;
using Core.Repositories;
using Core.Settings;
using Infrastructure.MongoDB;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests
{
    public class PoliceEventRepositoryTests
    {
        public PoliceEventRepositoryTests()
        {
        }

        [Fact]
        public async Task TestGetPoliceEventById()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEvent>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act
            //
            var res = await repo.GetEventById(2);

            //
            // Assert
            //
            mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<PoliceEvent>>(),
                                                   It.IsAny<FindOptions<PoliceEvent,
                                                   PoliceEvent>>(),
                                                   new CancellationToken()),
                                                   Times.Once);
        }

        [Fact]
        public async Task TestGetEventsForDate()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEvent>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act
            //
            var res = await repo.GetEventsForDate(DateTime.Now, DateTime.Now, null);
            var resList = res.ToList();

            //
            // Assert
            //
            mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<PoliceEvent>>(),
                                                   It.IsAny<FindOptions<PoliceEvent, PoliceEvent>>(),
                                                   new CancellationToken()),
                                                   Times.Once);
        }

        [Fact]
        public async Task TestUpsert()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEvent>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act
            //
            var list = new List<PoliceEvent>
            {
                new PoliceEvent { Id = 1, Summary = "summary1" },
                new PoliceEvent { Id = 2, Summary = "summary2" },
                new PoliceEvent { Id = 3, Summary = "summary3" }
            };
            await repo.UpsertCollection(list);

            //
            // Assert
            //
            mockCollection.Verify(op => op.ReplaceOneAsync(It.IsAny<FilterDefinition<PoliceEvent>>(),
                                                           It.IsAny<PoliceEvent>(),
                                                           It.IsAny<ReplaceOptions>(),
                                                           new CancellationToken()),
                                                           Times.Exactly(3));
        }


        private IMongoDBContext GetMockedContext(Mock<IMongoCollection<PoliceEvent>> mockCollection)
        {
            var cursor = new Mock<IAsyncCursor<PoliceEvent>>();

            var mockContext = new Mock<IMongoDBContext>();
            mockContext.Setup(c => c.GetCollection<PoliceEvent>("police_events")).Returns(mockCollection.Object);

            mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<PoliceEvent>>(),
                                                    It.IsAny<FindOptions<PoliceEvent, PoliceEvent>>(),
                                                    It.IsAny<CancellationToken>())).ReturnsAsync(cursor.Object);

            mockCollection.Setup(op => op.ReplaceOneAsync(It.IsAny<FilterDefinition<PoliceEvent>>(),
                                                          It.IsAny<PoliceEvent>(),
                                                          It.IsAny<ReplaceOptions>(),
                                                          new CancellationToken()));

            return mockContext.Object;
        }

        private IPoliceEventRepository GetRepository(IMongoDBContext context)
        {
            var settings = new RepositorySettings
            {
                PoliceEventCollectionName = "police_events"
            };
            var logger = new NullLogger<PoliceEventRepository>();
            var options = Options.Create(settings);

            return new PoliceEventRepository(options, logger, context);
        }

    }
}
