using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.CQRS.Queries;
using Core.Models;
using Core.Repositories;
using Moq;
using Xunit;

namespace Core.Tests
{
    public class GetPoliceEventsTests
    {
        [Fact]
        public async Task RepositoryShouldBeCalledWithParsedArguments()
        {
            //
            // Arrange
            //
            var repositoryMock = new Mock<IPoliceEventRepository>();
            var handler = new GetPoliceEvents.Handler(repositoryMock.Object);
            var query = new GetPoliceEvents.Query(
                new GetPoliceEvents.QueryParameters
                {
                    FromDate = "2020-01-01",
                    ToDate = "2020-01-02",
                    LocationName = "Place"
                }
             );

            //
            // Act
            //
            var res = await handler.Handle(query, new System.Threading.CancellationToken());

            //
            // Assert
            //
            var expFromDate = new DateTime(2020, 1, 1).Date;
            var expToDate = new DateTime(2020, 1, 2).Date;
            var expLocation = "Place";
            repositoryMock.Verify(a => a.GetEventsForDate(expFromDate, expToDate, expLocation), Times.Once);
        }

        [Fact]
        public async Task RepositoryShouldBeCalledWithDefaultArguments()
        {
            //
            // Arrange
            //
            var repositoryMock = new Mock<IPoliceEventRepository>();
            var handler = new GetPoliceEvents.Handler(repositoryMock.Object);
            var query = new GetPoliceEvents.Query(
                new GetPoliceEvents.QueryParameters()
             );

            //
            // Act
            //
            var res = await handler.Handle(query, new System.Threading.CancellationToken());

            //
            // Assert
            //
            var expDefaultDate = DateTime.Now.Date;
            repositoryMock.Verify(a => a.GetEventsForDate(expDefaultDate, expDefaultDate, null), Times.Once);
        }

        [Fact]
        public async Task TestFilterOutDistantEvents()
        {
            var repositoryMock = new Mock<IPoliceEventRepository>();
            repositoryMock.SetReturnsDefault(Task.FromResult(ResponseData()));
           
            var handler = new GetPoliceEvents.Handler(repositoryMock.Object);
            var query = new GetPoliceEvents.Query(
                new GetPoliceEvents.QueryParameters
                {
                    FromDate = "2020-01-01",
                    ToDate = "2020-01-02",
                    LocationName = "Place"
                }
             );

            var res = await handler.Handle(query, new System.Threading.CancellationToken());
            // No filter on distance, should return both items
            Assert.Equal(2, res.Count());

            query.Parameters.UserLat = 52.128;
            query.Parameters.UserLng = 12.459;
            query.Parameters.MaxDistanceKm = 300;
            res = await handler.Handle(query, new System.Threading.CancellationToken());
            // Large max distance, should return both items
            Assert.Equal(2, res.Count());

            query.Parameters.MaxDistanceKm = 200;
            res = await handler.Handle(query, new System.Threading.CancellationToken());
            // Smaller max distance, should return 1 item
            Assert.Single(res);

            query.Parameters.MaxDistanceKm = 0.5;
            res = await handler.Handle(query, new System.Threading.CancellationToken());
            // Very small max distance, should return no item
            Assert.Empty(res);
        }

        private IEnumerable<PoliceEvent> ResponseData()
        {
            return new List<PoliceEvent>
            {
                new PoliceEvent{Location = new Location{Lat=54.3567, Lng=13.123}},
                new PoliceEvent{Location = new Location{Lat=52.123, Lng=12.456}}
            };
        }
    }
}
