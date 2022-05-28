using Core.Clients;
using Core.Settings;
using Infrastructure.Clients;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests
{
    public class PoliceApiClientTests
    {
        private const string Data = @"[
            {
                'id': 1,
                'datetime': '2021-02-06 19:24:00 +01:00',
                'name': '06 februari 19:24, Bus, Stockholm',
                'summary': 'Bus på Åkergatan',
                'url': 'url1',
                'type': 'Bus, grov',
                'location': {
                    'name': 'Stockholm',
                    'gps': '59.329324,18.068581'
                }
            },
            {
                'id': 2,
                'datetime': '2021-02-06 17:24:00 +01:00',
                'name': '06 februari 17:24, Stöld, Örebro',
                'summary': 'Stöld på Hemgatan',
                'url': 'url2',
                'type': 'Stöld',
                'location': {
                    'name': 'Örebro',
                    'gps': '59.329334,18.068591'
                }
            }
            ]";

        [Fact]
        public async Task ShouldFetchEventsAndReturnConvertedData()
        {
            //
            // Arrange
            //
            var restClientMock = new Mock<IPoliceHttpClient>();
            restClientMock.Setup(a => a.GetAsync("myurl")).ReturnsAsync(() => new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(Data)
            });

            var settings = new PoliceApiSettings
            {
                PoliceApiUrl = "myurl"
            };
            var logger = new NullLogger<PoliceApiClient>();
            var options = Options.Create(settings);
            var client = new PoliceApiClient(options, logger, restClientMock.Object);

            //
            // Act
            //
            var events = (await client.GetLatestEvents()).ToList();

            //
            // Assert
            //
            restClientMock.Verify(m => m.GetAsync("myurl"), Times.Once);
            Assert.Equal(2, events.Count);

            Assert.Equal(new DateTime(2021, 2, 6, 18, 24, 0), events[0].UtcDateTime);
            Assert.Equal("Stockholm", events[0].Location.Name);
            Assert.Equal(59.329324, events[0].Location.Lat);
            Assert.Equal(18.068581, events[0].Location.Lng);
            Assert.Equal("Bus, grov", events[0].Type);

            Assert.Equal(new DateTime(2021, 2, 6, 16, 24, 0), events[1].UtcDateTime);
            Assert.Equal("Örebro", events[1].Location.Name);
            Assert.Equal(59.329334, events[1].Location.Lat);
            Assert.Equal(18.068591, events[1].Location.Lng);
            Assert.Equal("Stöld", events[1].Type);
        }
    }
}
