using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using IAG.Infrastructure.Middleware;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Middleware;

public class LoggingMiddlewareTest
{
    private readonly LoggingMiddleware _testMiddleware;
    private readonly MockILogger<LoggingMiddleware> _logger = new();
    private readonly Mock<RequestDelegate> _next = new();

    public LoggingMiddlewareTest()
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(_logger);
        _testMiddleware = new LoggingMiddleware(_next.Object, loggerFactory.Object);
    }

    [Fact]
    public async void InvokeTest()
    {
        var testRequest = "Hello World";
        var testUrl = new Uri("http://test.i-ag.ch:80/res/42?state=5&flag=true");
        var testContext = CreateTestContext(testUrl, testRequest);

        await _testMiddleware.Invoke(testContext);

        Assert.NotEmpty(_logger.LogEntries);
        Assert.All(_logger.LogEntries, Assert.NotEmpty);
        Assert.Contains("POST", _logger.LogEntries.First());
        Assert.Equal(2, _logger.LogEntries.Count);
        Assert.Contains(testUrl.OriginalString, _logger.LogEntries.First());
        Assert.Contains(testRequest, _logger.LogEntries.First());
        Assert.Contains($"'{HttpStatusCode.Accepted:G}'", _logger.LogEntries.Last());
        Assert.Contains($"({HttpStatusCode.Accepted:D})", _logger.LogEntries.Last());
    }

    [Fact]
    public async void InvokeWithBodyTest()
    {
        var testRequest = "Hello World";
        var testUrl = new Uri("http://test.i-ag.ch:80/res/42?state=5&flag=true");
        var testContext = CreateTestContext(testUrl, testRequest);
        testContext.Response.ContentType = ContentTypes.TextPlain;

        await _testMiddleware.Invoke(testContext);

        Assert.NotEmpty(_logger.LogEntries);
        Assert.All(_logger.LogEntries, Assert.NotEmpty);
        Assert.Contains("POST", _logger.LogEntries.First());
        Assert.Equal(2, _logger.LogEntries.Count);
        Assert.Contains(testUrl.OriginalString, _logger.LogEntries.First());
        Assert.Contains(testRequest, _logger.LogEntries.First());
    }

    [Fact]
    public async void NoLogging()
    {
        var testRequest = "Hello World";
        var testUrl = new Uri("http://test.i-ag.ch/");
        var testContext = CreateTestContext(testUrl, testRequest);
        _logger.EnabledState = false;
        await _testMiddleware.Invoke(testContext);

        Assert.Empty(_logger.LogEntries);
    }

    private static DefaultHttpContext CreateTestContext(Uri testUrl, string testRequest)
    {
        var testContext = new DefaultHttpContext();
        testContext.Request.Host = HostString.FromUriComponent(testUrl);
        testContext.Request.Path = PathString.FromUriComponent(testUrl);
        testContext.Request.Scheme = testUrl.Scheme;
        testContext.Request.ContentType = ContentTypes.TextPlain;
        testContext.Request.QueryString = QueryString.FromUriComponent(testUrl);
        testContext.Request.Method = "Post";
        testContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(testRequest));
        testContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
        return testContext;
    }
}