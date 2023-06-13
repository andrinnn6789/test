using System;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Rest;

namespace IAG.Infrastructure.Logging;

public static class MessageLoggerExtensions
{
    public static void LogException(this IMessageLogger logger, string name, System.Exception ex)
    {
        logger.AddMessage(MessageTypeEnum.Debug, name);
        LogException(logger, ex);
        logger.AddMessage(ex);
    }

    public static void LogException(this IMessageLogger logger, System.Exception ex)
    {
        while (ex != null)
        {
            if (ex is RestException restException)
            {
                if (restException.Content != null)
                {
                    logger.AddMessage(MessageTypeEnum.Debug, restException.Content);
                }

                foreach (var info in restException.AdditionalInfo)
                {
                    logger.AddMessage(MessageTypeEnum.Debug, info);
                }
            }
            else if (ex is LocalizableException locEx)
            {
                logger.AddMessage(MessageTypeEnum.Error, locEx.LocalizableParameter.ResourceId, locEx.LocalizableParameter.Params);
            }
            else if (ex is AggregateException aggEx)
            {
                foreach (var innerException in aggEx.InnerExceptions)
                {
                    LogException(logger, innerException);
                }
            }
            else
            {
                logger.AddMessage(MessageTypeEnum.Debug, ex.Message);
            }

            ex = ex.InnerException;
        }
    }
}