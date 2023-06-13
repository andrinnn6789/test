using System;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Middleware;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.Middleware;

public class ExceptionMiddlewareTest
{
    private readonly MockILogger<ExceptionMiddleware> _logger;
    private readonly IStringLocalizer<ExceptionMiddleware> _localizer;

    public ExceptionMiddlewareTest()
    {
        _logger = new MockILogger<ExceptionMiddleware>();
        var mockLocalizer = new Mock<IStringLocalizer<ExceptionMiddleware>>();
        mockLocalizer.Setup(_ => _[It.IsAny<string>()]).Returns((string s) => new LocalizedString(s, s));
        mockLocalizer.Setup(_ => _[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string s, object[] _) => new LocalizedString(s, s));
        _localizer = mockLocalizer.Object;
    }

    [Fact]
    public async Task InvokeNoExceptionTest()
    {
        var next = new Mock<RequestDelegate>();
        var testMiddleware = new ExceptionMiddleware(next.Object);

        await testMiddleware.Invoke(new DefaultHttpContext(), _logger, _localizer);

        Assert.Empty(_logger.LogEntries);
    }

    [Fact]
    public async Task InvokeSimpleExceptionTest()
    {
        var exceptionMsg = "Test-Exception";
        var exception = new System.Exception(exceptionMsg);

        var statusCode = await InvokeWitExceptionAsync(exception);

        Assert.Equal(500, statusCode);
        Assert.Equal(2, _logger.LogEntries.Count);
        Assert.Equal(exceptionMsg, _logger.LogEntries.First());
    }

    [Fact]
    public async Task InvokeLocalizableExceptionTest()
    {
        var exceptionMsgResId = "Test Exception {0}";
        var exceptionParam = 42;
        var exception = new LocalizableException(exceptionMsgResId, exceptionParam);
        var statusCode = await InvokeWitExceptionAsync(exception);

        Assert.Equal(500, statusCode);
        Assert.Equal(2, _logger.LogEntries.Count);
        Assert.Equal(exceptionMsgResId, _logger.LogEntries.First());
    }

    [Fact]
    public async void InvokeHttpExceptionTest()
    {
        var statusCodeBadRequest1 = await InvokeWitExceptionAsync(new BadRequestException(string.Empty));
        var statusCodeBadRequest2 = await InvokeWitExceptionAsync(new ArgumentNullException());
        var statusCodeBadRequest3 = await InvokeWitExceptionAsync(new ArgumentException());
        var statusCodeNotAuthenticated = await InvokeWitExceptionAsync(new AuthenticationFailedException(string.Empty));
        var statusCodeNotAuthorized = await InvokeWitExceptionAsync(new AuthorizationFailedException(string.Empty));
        var statusCodeNotFound = await InvokeWitExceptionAsync(new NotFoundException(string.Empty));
        var statusCodeConflict = await InvokeWitExceptionAsync(new DuplicateKeyException(string.Empty));

        Assert.Equal(400, statusCodeBadRequest1);
        Assert.Equal(400, statusCodeBadRequest2);
        Assert.Equal(400, statusCodeBadRequest3);
        Assert.Equal(401, statusCodeNotAuthenticated);
        Assert.Equal(403, statusCodeNotAuthorized);
        Assert.Equal(404, statusCodeNotFound);
        Assert.Equal(409, statusCodeConflict);
    }

    private async Task<int> InvokeWitExceptionAsync(System.Exception exception) 
    {
        var next = new RequestDelegate(_ => throw exception);
        var testMiddleware = new ExceptionMiddleware(next);
        var testContext = new DefaultHttpContext();

        await testMiddleware.Invoke(testContext, _logger, _localizer);

        return testContext.Response.StatusCode;
    }
}