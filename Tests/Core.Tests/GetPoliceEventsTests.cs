using System;
using System.Threading.Tasks;
using Core.CQRS.Queries;
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
    }
}
