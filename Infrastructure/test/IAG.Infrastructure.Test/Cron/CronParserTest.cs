using System;
using System.Globalization;

using IAG.Infrastructure.Cron;

using Xunit;

namespace IAG.Infrastructure.Test.Cron;

public class CronParserTests
{
    [Fact]
    public void Parse_WhenCronStringSet_ShouldReturnCron()
    {
        var result = CronParser.Parse("* * * * *");
        Assert.NotNull(result);
    }
    [Fact]
    public void Parse_WhenCronStringSet_ShouldReturnCronWithSeconds()
    {
        var result = CronParser.Parse("* * * * * *");
        Assert.NotNull(result);
    }

    [Theory, 
     InlineData(null), 
     InlineData(""), 
     InlineData("test data"), 
     InlineData("*"),
     InlineData("* *"),
     InlineData("* * *"),
     InlineData("* * * *")]
    public void Parse_WhenInvalidCronString_ParserShouldThrow(string cron)
    {
        Assert.Throws<CronParsingException>(() => CronParser.Parse(cron));
    }

    [Theory,
     InlineData("0 0 * * * ? *", "1/1/2000 00:00:00", "1/1/2000 01:00:00"),
     InlineData("0 * * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:01:00"),
     InlineData("* * * * * ? *", "1/1/2000 12:00:00", "1/1/2000 12:00:01")]
    public void Parse_WhenQuartzSyntax_ShouldConvertFromQuartzAndReturnCorrectly(string input, string startDate, string expectedDate)
    {
        CultureInfo culture = new CultureInfo("de-CH");
        var next = CronParser.Parse(input, Convert.ToDateTime(startDate), true);
        Assert.Equal(DateTime.Parse(expectedDate, culture), next);
    }

    [Theory,
     InlineData("0 */1 * * *", "1/1/2000 00:00:00", "1/1/2000 01:00:00"),
     InlineData("*/1 * * * *", "1/1/2000 12:00:00", "1/1/2000 12:01:00"),
     InlineData("* * * * * *", "1/1/2000 12:00:00", "1/1/2000 12:00:01")]
    public void Parse_WhenCronSyntax_ShouldReturnCorrectly(string input, string startDate, string expectedDate)
    {
        CultureInfo culture = new CultureInfo("de-CH");
        var next = CronParser.Parse(input, Convert.ToDateTime(startDate));
        Assert.Equal(DateTime.Parse(expectedDate, culture), next);
    }
    
    [Theory,
     InlineData("0 */1 * * *", "Jede Stunde"),
     InlineData("*/1 * * * *", "Jede Minute"),
     InlineData("* * * * * *", "Jede Sekunde")]
    public void GetHumanReadableFormat_ShouldReturnCorrectly(string input, string expected)
    {
        var result = CronParser.GetHumanReadableFormat(input);
        Assert.Equal(expected, result);
    }
}