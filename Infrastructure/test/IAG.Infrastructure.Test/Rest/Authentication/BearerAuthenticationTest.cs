using IAG.Infrastructure.Rest.Authentication;

using Xunit;

namespace IAG.Infrastructure.Test.Rest.Authentication;

public class BearerAuthenticationTest
{
    [Fact]
    public void BearerAuthenticationConstructorTest()
    {
        var auth = new BearerAuthentication("test_token");
        var authHeader = auth.GetAuthorizationHeader();

        Assert.NotNull(authHeader);
        Assert.Equal("Bearer", authHeader.Scheme);
    }
}