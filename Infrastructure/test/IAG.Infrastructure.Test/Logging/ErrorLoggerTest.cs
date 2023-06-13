using System.Net;

using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.xUnit;

using Xunit;

namespace IAG.Infrastructure.Test.Logging;

public class ErrorLoggerTest
{
    [Fact]
    public void LogTest()
    {
        var restEx = new RestException(HttpStatusCode.BadRequest, "test");
        restEx.AdditionalInfo.Add("add");
        var exWithInner = new System.Exception("inner", restEx);
        var errorLogger = new ErrorLogger();
        errorLogger.LogException(new MockILogger<ErrorLoggerTest>(), exWithInner);
    }
}