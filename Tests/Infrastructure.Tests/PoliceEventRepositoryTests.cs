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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests
{
    public class PoliceEventRepositoryTests
    {
        [Fact]
        public async Task TestGetPoliceEventById()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEventEntity>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act
            //
            _ = await repo.GetEventById(2);

            //
            // Assert
            //
            mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<PoliceEventEntity>>(),
                                                   It.IsAny<FindOptions<PoliceEventEntity,
                                                   PoliceEventEntity>>(),
                                                   new CancellationToken()),
                                                   Times.Once);
        }

        [Fact]
        public async Task TestGetEventsForDate_PageSizeLessThan1ShouldThrow()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEventEntity>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act & Assert
            //
            await Assert.ThrowsAsync<ArgumentException>(() => repo.GetEvents(DateTime.Now, DateTime.Now, 1, 0, 0, 0, 1));
        }

        [Fact]
        public async Task TestGetEventsForDate_PageLessThan1ShouldThrow()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEventEntity>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act & Assert
            //
            await Assert.ThrowsAsync<ArgumentException>(() => repo.GetEvents(DateTime.Now, DateTime.Now, 0, 1, 0, 0, 1));
        }

        /*
        [Fact]
        public async Task TestGetEventsForDate()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEventEntity>>();
            var context = GetMockedContext(mockCollection);
            var repo = GetRepository(context);

            //
            // Act
            //
            var res = await repo.GetEvents(DateTime.Now, DateTime.Now, 1, 5, 0, 0, 1, null);
            var resList = res.Events.ToList();

            //
            // Assert
            //
            mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<PoliceEventEntity>>(),
                                                   It.IsAny<FindOptions<PoliceEventEntity, PoliceEventEntity>>(),
                                                   new CancellationToken()),
                                                   Times.Once());
        }
        */

        [Fact]
        public async Task TestUpsert()
        {
            //
            // Arrange
            //
            var mockCollection = new Mock<IMongoCollection<PoliceEventEntity>>();
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
            mockCollection.Verify(op => op.ReplaceOneAsync(It.IsAny<FilterDefinition<PoliceEventEntity>>(),
                                                           It.IsAny<PoliceEventEntity>(),
                                                           It.IsAny<ReplaceOptions>(),
                                                           new CancellationToken()),
                                                           Times.Exactly(3));
        }


        private IMongoDbContext GetMockedContext(Mock<IMongoCollection<PoliceEventEntity>> mockCollection)
        {
            var cursor = new Mock<IAsyncCursor<PoliceEventEntity>>();
            var mockContext = new Mock<IMongoDbContext>();
            mockContext.Setup(c => c.GetCollection<PoliceEventEntity>("police_events")).Returns(mockCollection.Object);

            mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<PoliceEventEntity>>(),
                                                    It.IsAny<FindOptions<PoliceEventEntity, PoliceEventEntity>>(),
                                                    It.IsAny<CancellationToken>())).ReturnsAsync(cursor.Object);

            mockCollection.Setup(op => op.ReplaceOneAsync(It.IsAny<FilterDefinition<PoliceEventEntity>>(),
                                                          It.IsAny<PoliceEventEntity>(),
                                                          It.IsAny<ReplaceOptions>(),
                                                          new CancellationToken()));

            mockCollection.SetReturnsDefault(cursor);

            return mockContext.Object;
        }

        private IPoliceEventRepository GetRepository(IMongoDbContext context)
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
