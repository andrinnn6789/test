using System;

using IAG.Infrastructure.Atlas;

using Xunit;

namespace IAG.Infrastructure.Test.Atlas;

public class GuidConverterTest
{
    // base64-string from big-endian source
    private readonly string _base64 = "9rKwAB3aTXCXRd86aTdKWQ==";
    private readonly string _expectedBigEndianGuid = "f6b2b000-1dda-4d70-9745-df3a69374a59";
    private readonly string _expectedMixedEndianGuid = "00b0b2f6-da1d-704d-9745-df3a69374a59";

    [Fact]
    public void ToBigEndianGuidBytesTest()
    {
        var guidConverter = new GuidConverter();
        var bytes = Convert.FromBase64String(_base64);

        var bigEndianGuid = guidConverter.ToBigEndianGuid(bytes);
           
        Assert.Equal(_expectedBigEndianGuid, bigEndianGuid.ToString());
    }

    [Fact]
    public void ToBigEndianGuidBase64Test()
    {
        var guidConverter = new GuidConverter();

        var bigEndianGuid = guidConverter.ToBigEndianGuid(_base64);

        Assert.Equal(_expectedBigEndianGuid, bigEndianGuid.ToString());
    }

    [Fact]
    public void ToMixedEndianGuidBytesTest()
    {
        var guidConverter = new GuidConverter();
        var bytes = Convert.FromBase64String(_base64);

        var mixedEndianGuid = guidConverter.ToMixedEndianGuid(bytes);

        Assert.Equal(_expectedMixedEndianGuid, mixedEndianGuid.ToString());
    }

    [Fact]
    public void ToMixedEndianGuidBase64Test()
    {
        var guidConverter = new GuidConverter();

        var mixedEndianGuid = guidConverter.ToMixedEndianGuid(_base64);

        Assert.Equal(_expectedMixedEndianGuid, mixedEndianGuid.ToString());
    }
}