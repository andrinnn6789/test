using System;

using IAG.Infrastructure.Rest;

using Xunit;

namespace IAG.Infrastructure.Test.Rest;

public class UrlHelperTest
{
    [Fact]
    public void FullUrlHelperTest()
    {
        Assert.Empty(UrlHelper.Combine());
        Assert.Equal("Base/Part1/Part2", UrlHelper.Combine("Base", "Part1", "Part2"));
        Assert.Equal("Base/Part1/Part2", UrlHelper.Combine("Base/", "/Part1/", "Part2"));
        Assert.Equal("Base/Part1/Part2", UrlHelper.Combine("Base", "/Part1", "/Part2"));

        Assert.Throws<ArgumentNullException>(() => UrlHelper.Combine(null));
        Assert.Throws<ArgumentNullException>(() => UrlHelper.Combine("Test", null));
    }
}