using IAG.Infrastructure.Http;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;

namespace IAG.Infrastructure.Test.Http;

public class HttpContextHelperTest
{
    [Fact]
    public void IsLocalRequestShouldRecognizeLocalRequestTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = null;
        httpContext.Connection.LocalIpAddress = null;

        Assert.True(HttpContextHelper.IsLocalRequest(httpContext));
    }

    [Fact]
    public void IsLocalRequestShouldRecognizeLoopbackRequestTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Loopback;
        httpContext.Connection.LocalIpAddress = IPAddress.Loopback;

        Assert.True(HttpContextHelper.IsLocalRequest(httpContext));
    }

    [Fact]
    public void IsLocalRequestShouldRecognizeIPv6LoopbackRequestTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.IPv6Loopback;
        httpContext.Connection.LocalIpAddress = IPAddress.IPv6Loopback;

        Assert.True(HttpContextHelper.IsLocalRequest(httpContext));
    }

    [Fact]
    public void IsLocalRequestShouldRecognizeRemoteRequestTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("123.4.5.6");
        httpContext.Connection.LocalIpAddress = IPAddress.Loopback;

        Assert.False(HttpContextHelper.IsLocalRequest(httpContext));
    }

    [Fact]
    public void IsLocalRequestShouldRecognizeForwardHeaderTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Loopback;
        httpContext.Connection.LocalIpAddress = IPAddress.Loopback;
        httpContext.Request.Headers["x-forwarded-for"] = "ForTest";

        Assert.False(HttpContextHelper.IsLocalRequest(httpContext));
    }
}