using Core.Helpers;
using System;
using Xunit;

namespace Core.Tests
{
    public class DateHelperTests
    {
        [Fact]
        public void ShouldReturnParseDate()
        {
            var defaultDate = new DateTime(2020, 10, 2);
            var parsed = DateHelper.GetParsedDateOrDefault("2021-03-12", defaultDate);
            Assert.Equal(new DateTime(2021, 3, 12), parsed);
        }

        [Fact]
        public void ShouldReturnDefaultDateOnUnparsableDateString()
        {
            var defaultDate = new DateTime(2020, 10, 2);
            var parsed = DateHelper.GetParsedDateOrDefault("abc", defaultDate);
            Assert.Equal(defaultDate, parsed);

            parsed = DateHelper.GetParsedDateOrDefault("200312", defaultDate);
            Assert.Equal(defaultDate, parsed);

            parsed = DateHelper.GetParsedDateOrDefault("20201002", defaultDate);
            Assert.Equal(defaultDate, parsed);

            parsed = DateHelper.GetParsedDateOrDefault("", defaultDate);
            Assert.Equal(defaultDate, parsed);

            parsed = DateHelper.GetParsedDateOrDefault(null, defaultDate);
            Assert.Equal(defaultDate, parsed);
        }
    }
}
