using Infrastructure.Clients;
using System;
using Xunit;

namespace Infrastructure.Tests
{
    public class ModelConversionTests
    {
        [Fact]
        public void ShouldConvertFromExternalModel()
        {
            //
            // Arrange
            //
            var extData = new PoliceEventExternal
            {
                Datetime = "2021-02-06 19:24:00 +01:00",
                Location = new LocationExternal
                {
                    Gps = "10.1234,11.4567",
                    Name = "MunicipalityA"
                },
                Summary = "the summary",
                Id = 42,
                Type = "event type",
                Url = "the url"
            };

            //
            // Act
            //
            var policeEvent = extData.GetPoliceEvent();

            //
            // Assert
            //
            Assert.Equal(10.1234, policeEvent.Location.Lat);
            Assert.Equal(11.4567, policeEvent.Location.Lng);
            Assert.Equal("MunicipalityA", policeEvent.Location.Name);
            Assert.Equal(new DateTime(2021, 2, 6, 18, 24, 0), policeEvent.UtcDateTime);
            Assert.Equal("the summary", policeEvent.Summary);
            Assert.Equal(42, policeEvent.Id);
            Assert.Equal("event type", policeEvent.Type);
            Assert.Equal("the url", policeEvent.Url);
        }
    }
}
