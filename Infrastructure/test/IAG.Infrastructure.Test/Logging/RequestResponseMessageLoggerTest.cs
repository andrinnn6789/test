using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Logging;

public class RequestResponseMessageLoggerTest
{
    private readonly ILogger _iLogger;
    private readonly IMessageLogger _messageLogger;
    private readonly RequestResponseLogger _logger;
    private readonly List<MessageStructure> _messages;
    private readonly List<string> _logEntries;

    public RequestResponseMessageLoggerTest()
    {
        _logEntries = new List<string>();
        _iLogger = new MockILogger<object>(_logEntries);

        _messages = new List<MessageStructure>();
        var mockMessageLogger = new Mock<IMessageLogger>();
        mockMessageLogger.Setup(m => m.AddMessage(It.IsAny<MessageStructure>())).Callback<MessageStructure>(msg => _messages.Add(msg));
        _messageLogger = mockMessageLogger.Object;

        _logger = new RequestResponseLogger(_iLogger, _messageLogger);
    }

    [Fact]
    public void LogGetRequestTest()
    {
        var testUrl = "http://test.i-ag.ch/hello/world?value=23&flag=true";
        var request = new HttpRequestMessage(HttpMethod.Get, testUrl);

        _logger.LogRequest(null, request);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.NotNull(message.Params);
        Assert.All(message.Params, Assert.NotNull);
        Assert.Single(message.Params);
        Assert.Contains(testUrl, message.Params[0].ToString() ?? string.Empty);

        var logEntry = Assert.Single(_logEntries);
        Assert.NotNull(logEntry);
        Assert.Contains(testUrl, logEntry);
        Assert.Contains(HttpMethod.Get.ToString(), logEntry);
    }

    [Fact]
    public void LogGetRequestWithHeaderTest()
    {
        var testUrl = "http://test.i-ag.ch/hello/world?value=23&flag=true";
        var testMimeType = "test/mime-type";
        var request = new HttpRequestMessage(HttpMethod.Get, testUrl);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(testMimeType));

        _logger.LogRequest(null, request);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.NotNull(message.Params);
        Assert.All(message.Params, Assert.NotNull);
        Assert.Single(message.Params);
        Assert.Contains(testUrl, message.Params[0].ToString() ?? string.Empty);

        var logEntry = Assert.Single(_logEntries);
        Assert.NotNull(logEntry);
        Assert.Contains(testUrl, logEntry);
        Assert.Contains(testMimeType, logEntry);
        Assert.Contains(HttpMethod.Get.ToString(), logEntry);
    }

    [Fact]
    public void LogPostRequestTest()
    {
        var testUrl = "http://test.i-ag.ch/hello/world";
        var testContent = "Hello World";
        var request = new HttpRequestMessage(HttpMethod.Post, testUrl)
        {
            Content = new StringContent(testContent, Encoding.UTF8, ContentTypes.TextPlain)
        };


        _logger.LogRequest(null, request);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.NotNull(message.Params);
        Assert.All(message.Params, Assert.NotNull);
        Assert.Single(message.Params);
        Assert.Contains(testUrl, message.Params[0].ToString() ?? string.Empty);
        Assert.Contains(testContent, message.Params[0].ToString() ?? string.Empty);

        var logEntry = Assert.Single(_logEntries);
        Assert.NotNull(logEntry);
        Assert.Contains(testUrl, logEntry);
        Assert.Contains(HttpMethod.Post.ToString(), logEntry);
        Assert.Contains(testContent, logEntry);
    }

    [Fact]
    public void LogPostBinMediaTypeRequestTest()
    {
        var testUrl = "http://test.i-ag.ch/hello/world";
        var testContent = "Hello World";
        var request = new HttpRequestMessage(HttpMethod.Post, testUrl)
        {
            Content = new StringContent(testContent, Encoding.UTF8, ContentTypes.ApplicationPdf)
        };

        _logger.LogRequest(null, request);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.NotNull(message.Params);
        Assert.All(message.Params, Assert.NotNull);
        Assert.Single(message.Params);
        Assert.Contains(testUrl, message.Params[0].ToString() ?? string.Empty);
        Assert.DoesNotContain(testContent, message.Params[0].ToString() ?? string.Empty);
    }

    [Fact]
    public void LogResponseTest()
    {
        var testCode = HttpStatusCode.Conflict;
        var testContent = "Bad test result";
        var testHeader = "X-Test-Header";
        var testHeaderValues = new[] { "Hello", "World" };
        var response = new HttpResponseMessage(testCode)
        {
            Content = new StringContent(testContent, Encoding.UTF8, ContentTypes.TextPlain)
        };
        response.Headers.Add(testHeader, testHeaderValues);

        _logger.LogResponse(response);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.NotNull(message.Params);
        Assert.All(message.Params, Assert.NotNull);
        Assert.Single(message.Params);
        Assert.Contains(testCode.ToString(), message.Params[0].ToString() ?? string.Empty);
        Assert.Contains(((int)testCode).ToString(), message.Params[0].ToString() ?? string.Empty);
        Assert.Contains(testHeader, message.Params[0].ToString() ?? string.Empty);

        var logEntry = Assert.Single(_logEntries);
        Assert.NotNull(logEntry);
        Assert.Contains(testCode.ToString(), logEntry);
        Assert.Contains(testHeader, logEntry);
        Assert.All(testHeaderValues, hv => Assert.Contains(hv, logEntry));
        Assert.Contains(testContent, logEntry);
    }

    [Fact]
    public void LogBinResponseTest()
    {
        var testCode = HttpStatusCode.Conflict;
        var testContent = "Bad test result";
        var response = new HttpResponseMessage(testCode)
        {
            Content = new StringContent(testContent, Encoding.UTF8, ContentTypes.ApplicationPdf)
        };

        _logger.LogResponse(response);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.NotNull(message.Params);
        Assert.All(message.Params, Assert.NotNull);
        Assert.Single(message.Params);
        Assert.DoesNotContain(testContent, message.Params[0].ToString() ?? string.Empty);
    }

    [Fact]
    public void LoggerOnlyTest()
    {
        var logger = new RequestResponseLogger(_iLogger);
        var testUrl = "http://test.i-ag.ch/hello/world?value=23&flag=true";
        var request = new HttpRequestMessage(HttpMethod.Get, testUrl);

        logger.LogRequest(null, request);

        var logEntry = Assert.Single(_logEntries);
        Assert.NotNull(logEntry);
        Assert.Empty(_messages);
    }

    [Fact]
    public void MessageLoggerOnlyTest()
    {
        var logger = new RequestResponseLogger(_messageLogger);
        var testUrl = "http://test.i-ag.ch/hello/world?value=23&flag=true";
        var request = new HttpRequestMessage(HttpMethod.Get, testUrl);

        logger.LogRequest(null, request);

        var message = Assert.Single(_messages);
        Assert.NotNull(message);
        Assert.Empty(_logEntries);
    }

    [Fact]
    public void IsContentText()
    {
        Assert.True(RequestResponseLogger.IsContentText("x-application/json;charset=UTF-8"));
        Assert.True(RequestResponseLogger.IsContentText("application/json"));
        Assert.False(RequestResponseLogger.IsContentText(null));
        Assert.False(RequestResponseLogger.IsContentText(""));
        Assert.False(RequestResponseLogger.IsContentText("x-"));
        Assert.False(RequestResponseLogger.IsContentText("x-asas"));
    }
}