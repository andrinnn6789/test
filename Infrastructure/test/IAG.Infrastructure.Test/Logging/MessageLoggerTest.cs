using System;
using System.Collections.Generic;
using System.Net;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Logging;

public class MessageLoggerTest
{
    private readonly IMessageLogger _logger;
    private readonly List<string> _logData = new();
    public MessageLoggerTest()
    {
        var mockLog = new Mock<IMessageLogger>();
        mockLog.Setup(m => m.AddMessage(It.IsAny<MessageTypeEnum>(), It.IsAny<string>(), It.IsAny<object[]>()))
            .Callback<MessageTypeEnum, string, object[]>((_, msg, _) => _logData.Add(msg));
        _logger = mockLog.Object;
    }

    [Fact]
    public void LogBasicTest()
    {
        var restEx = new RestException(HttpStatusCode.BadRequest, "test");
        restEx.AdditionalInfo.Add("add");
        _logger.LogException("hallo", restEx);

        Assert.Equal(3, _logData.Count);
    }

    [Fact]
    public void LogAggregateTest()
    {
        var aggEx = new AggregateException(
            new RestException(HttpStatusCode.BadRequest, "test"), 
            new LocalizableException("resId", "test"), 
            new System.Exception("ex")
        );
        _logger.LogException("hallo", aggEx);

        Assert.Equal(5, _logData.Count);
    }
}