using System.Collections.Generic;
using System.Text;

using IAG.Infrastructure.Formatter;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.Infrastructure.Test.Formatter;

public class JsonToCsvTest
{
    [Fact]
    public void ConvertEmptyTest()
    {
        var converter = new JsonToCsv();
        var testJson = new List<JObject>();
        var expectedCsv = string.Empty;

        string csv = converter.ConvertToCsv(testJson);

        Assert.Equal(expectedCsv, csv);
    }

    [Fact]
    public void ConvertSimpleTest()
    {
        var converter = new JsonToCsv();
        var testJson = new List<JObject>() { JObject.Parse(@"{a:42, b:""Hallo Welt""}"), JObject.Parse(@"{a:23, b:""foobar""}") };
        var expectedCsv = new StringBuilder();
        expectedCsv.AppendLine($"a{converter.Delimiter}b");
        expectedCsv.AppendLine($"42{converter.Delimiter}Hallo Welt");
        expectedCsv.AppendLine($"23{converter.Delimiter}foobar");

        string csv = converter.ConvertToCsv(testJson);

        Assert.Equal(expectedCsv.ToString(), csv);
    }

    [Fact]
    public void ConvertWithEscapingTest()
    {
        var converter = new JsonToCsv();
        var testJson = new List<JObject>() { JObject.Parse(@"{a:42, b:""Hallo \""Welt\""""}"), JObject.Parse(@"{a:23, b:""foobar;""}") };
        var expectedCsv = new StringBuilder();
        expectedCsv.AppendLine($"a{converter.Delimiter}b");
        expectedCsv.AppendLine($"42{converter.Delimiter}\"Hallo \\\"Welt\\\"\"");
        expectedCsv.AppendLine($"23{converter.Delimiter}\"foobar;\"");

        string csv = converter.ConvertToCsv(testJson);

        Assert.Equal(expectedCsv.ToString(), csv);
    }

    [Fact]
    public void ConvertWithCustomDelimiterTest()
    {
        var converter = new JsonToCsv('&');
        var testJson = new List<JObject>() { JObject.Parse(@"{a:42, b:""Hallo Welt""}"), JObject.Parse(@"{a:23, b:""foobar""}") };
        var expectedCsv = new StringBuilder();
        expectedCsv.AppendLine($"a{converter.Delimiter}b");
        expectedCsv.AppendLine($"42{converter.Delimiter}Hallo Welt");
        expectedCsv.AppendLine($"23{converter.Delimiter}foobar");

        string csv = converter.ConvertToCsv(testJson);

        Assert.Equal(expectedCsv.ToString(), csv);
    }

    [Fact]
    public void ConvertWithCustomDelimiterAndEscapingTest()
    {
        var converter = new JsonToCsv('&');
        var testJson = new List<JObject>() { JObject.Parse(@"{a:23, b:""foo&bar""}") };
        var expectedCsv = new StringBuilder();
        expectedCsv.AppendLine($"a{converter.Delimiter}b");
        expectedCsv.AppendLine($"23{converter.Delimiter}\"foo&bar\"");

        string csv = converter.ConvertToCsv(testJson);

        Assert.Equal(expectedCsv.ToString(), csv);
    }
}