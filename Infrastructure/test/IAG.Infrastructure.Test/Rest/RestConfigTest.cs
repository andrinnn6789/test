using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

using Xunit;

#pragma warning disable 618
namespace IAG.Infrastructure.Test.Rest;

public class TestConfigTest
{
    [Fact]
    public void ObsoleteConfigTest()
    {
        var user = "test_user";
        var password = "test_password";

        var config3 = new HttpConfig();
        config3.Authentication = new BasicAuthentication() { User = user, Password = password };

        var auth3 = config3.Authentication?.GetAuthorizationHeader();

        Assert.NotNull(auth3);
        Assert.Equal(user, ((BasicAuthentication)config3.Authentication).User);
        Assert.Equal(password, ((BasicAuthentication)config3.Authentication).Password);
    }
}