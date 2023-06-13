using System.Text;

using IAG.Infrastructure.Atlas;

using Xunit;

namespace IAG.Infrastructure.Test.Atlas;

public class XmlCleanerTest
{
    [Fact]
    public void ToBigEndianGuidBytesTest()
    {
        var invalidString = @"abc" + char.ConvertFromUtf32(1) + "_" + char.ConvertFromUtf32(18);
        var validString = @"abc_";
        var validXmlFromInvalid = XmlCleaner.Serialize(invalidString, Encoding.UTF8);
        var validXmlFromValid = XmlCleaner.Serialize(validString, Encoding.UTF8);

        Assert.Equal(validXmlFromValid, validXmlFromInvalid);
    }
}