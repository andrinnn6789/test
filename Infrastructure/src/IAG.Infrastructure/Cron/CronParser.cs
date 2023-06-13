using System;

using CronExpressionDescriptor;

using NCrontab;

namespace IAG.Infrastructure.Cron;

public static class CronParser
{
    public static DateTime? Parse(string cronString, DateTime? baseTime = null, bool? quartzConvert = false)
    {
        if (string.IsNullOrEmpty(cronString))
            throw new CronParsingException("crontab expression cannot be null");

        if (!baseTime.HasValue)
        {
            baseTime = DateTime.Now;
        }

        if (quartzConvert.HasValue && quartzConvert.Value)
            cronString = ConvertFromQuartz(cronString);

        try
        {
            return CrontabSchedule.Parse(cronString, CronTabSchedulerOptions(cronString)).GetNextOccurrence(baseTime.Value);
        }
        catch (CrontabException ex)
        {
            var cronParsingException = new CronParsingException($"{cronString} is a invalid crontab expression", ex);
            cronParsingException.Data.Add("CronTabExpression", cronString);
            throw cronParsingException;
        }
    }
    
    public static string GetHumanReadableFormat(string cronExpression)
    {
        var options = new Options
        {
            Locale = "de-CH",
            Use24HourTimeFormat = true
        };

        return ExpressionDescriptor.GetDescription(cronExpression, options);
    }

    private static string ConvertFromQuartz(string cronString)
    {
        var validCronString = cronString;

        if (cronString.Split(' ').Length == 7)
        {
            var splitCronExpression = cronString.Split(' ');
            validCronString = "";
            for (var i = 0; i < splitCronExpression.Length - 1; i++)
            {
                if (splitCronExpression[i] == "?")
                    splitCronExpression[i] = "*";
                if (i < 7)
                {
                    validCronString += splitCronExpression[i] + ' ';
                }
            }
            validCronString = validCronString.Trim();
        }

        return validCronString;
    }

    private static CrontabSchedule.ParseOptions CronTabSchedulerOptions(string cronString)
    {
        return new()
        {
            IncludingSeconds = cronString.Split(' ').Length > 5
        };
    }
}