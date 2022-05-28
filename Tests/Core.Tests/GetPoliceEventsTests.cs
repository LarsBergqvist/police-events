using Core.CQRS.Queries;
using Core.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
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
                    LocationName = "Place",
                    Page = 3,
                    PageSize = 7,
                    UserLat = 123.456,
                    UserLng = 456.123,
                    MaxDistanceKm = 17.1
                }
             );

            //
            // Act
            //
            _ = await handler.Handle(query, new System.Threading.CancellationToken());

            //
            // Assert
            //
            var expFromDate = new DateTime(2020, 1, 1).Date;
            var expToDate = new DateTime(2020, 1, 2).Date;
            var expLocation = "Place";
            repositoryMock.Verify(a => a.GetEvents(expFromDate, expToDate,
                3, 7,
                123.456, 456.123, 17.1,
                expLocation, null), Times.Once);
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
            _ = await handler.Handle(query, new System.Threading.CancellationToken());

            //
            // Assert
            //
            var expDefaultDate = DateTime.Now.Date;
            repositoryMock.Verify(a => a.GetEvents(expDefaultDate, expDefaultDate,
                GetPoliceEvents.DefaultPage, GetPoliceEvents.DefaultPageSize,
                0, 0, GetPoliceEvents.DefaultMaxDistKm,
                null, null), Times.Once);
        }
    }
}
