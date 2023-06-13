using System;

namespace IAG.Infrastructure.Cron;

public class CronParsingException : ApplicationException
{
    public CronParsingException(string message)
        : base(message)
    {
    }

    public CronParsingException(string message, System.Exception innerException) :
        base(message, innerException)
    {
    }
}