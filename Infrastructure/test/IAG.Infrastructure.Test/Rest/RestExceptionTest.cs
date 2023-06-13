using System.Net;

using IAG.Infrastructure.Rest;

using Xunit;

namespace IAG.Infrastructure.Test.Rest;

public class RestExceptionTest
{
    [Fact]
    public void ConstructorTest()
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var content = "Test-Error";
        var testException = new RestException(statusCode, content);

        Assert.NotNull(testException);
        Assert.Equal(statusCode, testException.StatusCode);
        Assert.Equal(content, testException.Content);
    }

    [Fact]
    public void ConstructorWithInnerExceptionTest()
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var content = "Test-Error";
        var testException = new RestException(statusCode, content);

        Assert.NotNull(testException);
        Assert.Equal(statusCode, testException.StatusCode);
        Assert.Equal(content, testException.Content);
    }
}