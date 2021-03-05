using Core.Helpers;
using Xunit;

namespace Core.Tests
{
    public class DistanceCalculatorTests
    {
        [Fact]
        public void TestLargeDistance()
        {
            var lat1 = 54.3567;
            var lng1 = 13.123;
            var lat2 = 52.123;
            var lng2 = 12.456;
            var dist = DistanceCalculator.GetDistanceKm(lat1, lng1, lat2, lng2);
            Assert.True(dist > 251 && dist < 253);
        }

        [Fact]
        public void TestShortDistance()
        {
            var lat1 = 59.517513;
            var lng1 = 17.644196;
            var lat2 = 59.558462;
            var lng2 = 17.541561;
            var dist = DistanceCalculator.GetDistanceKm(lat1, lng1, lat2, lng2);
            Assert.True(dist > 6.5 && dist < 7.5);
        }
    }
}
