using Core.Helpers;
using Xunit;

namespace Core.Tests
{
    public class UrlHelperTests
    {
        [Theory]
        [InlineData("/abc/123", "https://polisen.se/abc/123")]
        [InlineData("https://polisen.se/abc/123", "https://polisen.se/abc/123")]
        [InlineData("abc/123", "https://polisen.se/abc/123")]
        [InlineData("", "https://polisen.se")]
        [InlineData(null, "https://polisen.se")]
        public void UrlShouldBePrefixedCorrectly(string input, string expectedResult)
        {
            Assert.Equal(expectedResult, UrlHelper.CompleteEventUrl(input));
        }

    }
}
