using IAG.Infrastructure.Rest.Authentication;

using Xunit;

namespace IAG.Infrastructure.Test.Rest.Authentication;

public class BasicAuthenticationTest
{
    [Fact]
    public void BasicAuthenticationConstructorTest()
    {
        var auth = new BasicAuthentication() { User = "test_user", Password = "test_password" };
        var authHeader = auth.GetAuthorizationHeader();

        Assert.NotNull(authHeader);
        Assert.Equal("Basic", authHeader.Scheme);
    }
}