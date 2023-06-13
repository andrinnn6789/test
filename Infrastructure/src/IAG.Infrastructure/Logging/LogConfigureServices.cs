using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Settings;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Config;
using NLog.Extensions.Logging;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace IAG.Infrastructure.Logging;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class LogConfigureServices
{
    public static void ConfigLogging(ILoggingBuilder logging, IConfiguration config)
    {
        logging.ClearProviders();
        // Add some default providers...
        logging.AddDebug();
        logging.AddEventSourceLogger();
        logging.SetMinimumLevel(LogLevel.Trace);

        if (config.GetSection("Logging:Console").Exists())
        {
            logging.AddConsole();
        }
        if (config.GetSection("Logging:EventLog").Exists())
        {
            logging.AddEventLog();
        }
        AddNLog(logging);
    }

    private static void AddNLog(ILoggingBuilder logging)
    {
        LogManager.ThrowExceptions = true;
        try
        {
            logging.AddNLog();
            var loggingConfiguration = new XmlLoggingConfiguration(
                new SettingsFinder().GetSettingsFilePath("NLog.config"));
            LogManager.Configuration = loggingConfiguration;
            LogManager.ThrowExceptions = false;
        }
        catch (System.Exception e)
        {
            ErrorToConsole(e);
        }
    }

    private static void ErrorToConsole(System.Exception messgae)
    {
        var orgColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        while (messgae != null)
        {
            Console.WriteLine(messgae.Message);
            messgae = messgae.InnerException;
        }

        Console.ForegroundColor = orgColor;
    }
}