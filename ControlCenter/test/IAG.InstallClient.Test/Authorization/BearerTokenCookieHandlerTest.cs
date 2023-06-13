using IAG.InstallClient.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Xunit;

namespace IAG.InstallClient.Test.Authorization;

public class BearerTokenCookieHandlerTest
{
    [Fact]
    public void GetBearerTokenTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = new[] { "Bearer-Token=ValidBearerToken" };

        var token = BearerTokenCookieHandler.GetBearerToken(httpContext);

        Assert.Equal("ValidBearerToken", token);
    }

    [Fact]
    public void SetBearerTokenTest()
    {
        var httpContext = new DefaultHttpContext();
        BearerTokenCookieHandler.SetBearerToken(httpContext, "ValidBearerToken");

        var tokenCookie = httpContext.Response.GetTypedHeaders().SetCookie.FirstOrDefault();
        Assert.NotNull(tokenCookie);
        Assert.Equal("ValidBearerToken", tokenCookie.Value);
    }

    [Fact]
    public void ClearBearerTokenTest()
    {
        var httpContext = new DefaultHttpContext();
        BearerTokenCookieHandler.ClearBearerToken(httpContext);

        var tokenCookie = httpContext.Response.GetTypedHeaders().SetCookie.FirstOrDefault();
        Assert.NotNull(tokenCookie);
        Assert.True(DateTime.UtcNow > tokenCookie.Expires);
        Assert.Equal(0, tokenCookie.Value.Length);
    }
}