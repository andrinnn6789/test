using System.Collections.Generic;
using System.Text;

using IAG.Infrastructure.Formatter;

using JetBrains.Annotations;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.Infrastructure.Test.Formatter;

public class CsvOutputFormatterTest
{
    [Fact]
    public void FormatEmptyJObjectList()
    {
        var formatter = new CsvOutputFormatter();
        var testResponse = new List<JObject>();
        var expectedCsv = string.Empty;

        string csv = formatter.ConvertToCsv(testResponse);

        Assert.Equal(expectedCsv, csv);
    }

    [Fact]
    public void FormatEmptyStringList()
    {
        var formatter = new CsvOutputFormatter();
        var testResponse = new List<string>();
        var expectedCsv = string.Empty;

        string csv = formatter.ConvertToCsv(testResponse);

        Assert.Equal(expectedCsv, csv);
    }

    [Fact]
    public void FormatJObjectList()
    {
        var formatter = new CsvOutputFormatter();
        var testResponse = new List<JObject>() { JObject.Parse(@"{a:42, b:""Hallo Welt""}"), JObject.Parse(@"{a:23, b:""foobar""}") };
        var expectedCsv = new StringBuilder();
        expectedCsv.AppendLine($"a{JsonToCsv.DefaultDelimiter}b");
        expectedCsv.AppendLine($"42{JsonToCsv.DefaultDelimiter}Hallo Welt");
        expectedCsv.AppendLine($"23{JsonToCsv.DefaultDelimiter}foobar");

        string csv = formatter.ConvertToCsv(testResponse);

        Assert.Equal(expectedCsv.ToString(), csv);
    }

    [Fact]
    public void FormatTestObjectList()
    {
        var formatter = new CsvOutputFormatter();
        var testResponse = new List<Test>() { new() { TestNumber = 42, TestString = "Hallo Welt" }, new() { TestNumber = 23, TestString = "foobar" } };
        var expectedCsv = new StringBuilder();
        expectedCsv.AppendLine($"TestNumber{JsonToCsv.DefaultDelimiter}TestString");
        expectedCsv.AppendLine($"42{JsonToCsv.DefaultDelimiter}Hallo Welt");
        expectedCsv.AppendLine($"23{JsonToCsv.DefaultDelimiter}foobar");

        string csv = formatter.ConvertToCsv(testResponse);

        Assert.Equal(expectedCsv.ToString(), csv);
    }

    private class Test
    {
        [UsedImplicitly]
        public int TestNumber { get; set; }

        [UsedImplicitly]
        public string TestString { get; set; }
    }
}