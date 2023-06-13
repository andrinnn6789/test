using IAG.Infrastructure.Rest;

using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.Logging;

public class ErrorLogger
{
    public void LogException(ILogger logger, System.Exception e)
    {
        logger.LogError(e.Message);
        if (e.InnerException == null)
            logger.LogDebug(e.StackTrace);
        if (e is RestException re)
        {
            if (re.Content != null)
                logger.LogDebug(re.Content);
            foreach (var info in re.AdditionalInfo)
            {
                logger.LogDebug(info);
            }
        }
        if (e.InnerException != null)
            LogException(logger, e.InnerException);
    }
}