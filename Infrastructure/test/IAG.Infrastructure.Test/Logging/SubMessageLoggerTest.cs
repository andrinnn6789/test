using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using Moq;
using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Model;

using Xunit;

namespace IAG.Infrastructure.Test.Logging;

public class SubMessageLoggerTest
{
    private readonly IMessageLogger _logger;
    private readonly List<string> _logData = new();
    private double _reportedProgress;

    public SubMessageLoggerTest()
    {
        var mockLog = new Mock<IMessageLogger>();
        mockLog.Setup(m => m.AddMessage(It.IsAny<MessageTypeEnum>(), It.IsAny<string>(), It.IsAny<object[]>()))
            .Callback<MessageTypeEnum, string, object[]>((_, msg, _) => _logData.Add(msg));

        mockLog.Setup(m => m.ReportProgress(It.IsAny<double>()))
            .Callback<double>(progress => _reportedProgress = progress);
        _logger = mockLog.Object;
    }

    [Fact]
    public void LogMessagesTest()
    {
        var subMsgLogger = new SubMessageLogger(_logger, 0, 1);

        subMsgLogger.AddMessage(MessageTypeEnum.Debug, "Test");
        subMsgLogger.AddMessage(new System.Exception());
        subMsgLogger.AddMessage(new MessageStructure(MessageTypeEnum.Debug, "Test"));

        Assert.NotEmpty(_logData);
    }

    [Fact]
    public void ProgressRangeTest()
    {
        var subMsgLogger = new SubMessageLogger(_logger, 0.2, 0.5);

        subMsgLogger.ReportProgress(0);
        var progress0 = _reportedProgress;
        subMsgLogger.ReportProgress(0.5);
        var progress50 = _reportedProgress;
        subMsgLogger.ReportProgress(1);
        var progress100 = _reportedProgress;

        Assert.Equal(0.2, progress0);
        Assert.Equal(0.35, progress50);
        Assert.Equal(0.5, progress100);
    }
}