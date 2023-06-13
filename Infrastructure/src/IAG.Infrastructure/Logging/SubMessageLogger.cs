using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;

namespace IAG.Infrastructure.Logging;

public class SubMessageLogger : IMessageLogger
{
    private readonly IMessageLogger _parentMessageLogger;
    private readonly double _progressRangeFrom;
    private readonly double _progressRangeFactor;

    public SubMessageLogger(IMessageLogger parentMessageLogger, double progressRangeFrom, double progressRangeTo)
    {
        _parentMessageLogger = parentMessageLogger;
        _progressRangeFrom = progressRangeFrom;
        _progressRangeFactor = (progressRangeTo - progressRangeFrom);
    }

    public void AddMessage(MessageTypeEnum type, string resourceId, params object[] args)
        => _parentMessageLogger.AddMessage(type, resourceId, args);

    public void AddMessage(MessageStructure message)
        => _parentMessageLogger.AddMessage(message);

    public void AddMessage(System.Exception e)
        => _parentMessageLogger.AddMessage(e);

    public void ReportProgress(double progress)
    {
        _parentMessageLogger.ReportProgress(_progressRangeFrom + progress*_progressRangeFactor);
    }
}