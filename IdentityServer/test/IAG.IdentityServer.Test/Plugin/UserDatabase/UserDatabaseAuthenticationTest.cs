using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Logic;

using Xunit;

namespace IAG.IdentityServer.Test.Plugin.UserDatabase;

public class UserDatabaseAuthenticationTest
{
    [Fact]
    public void AuthenticationTest()
    {
        var authLogic = new UserDatabaseAuthentication();
        var user = new User
        {
            Name = "TestUser"
        };

        authLogic.UpdatePassword(user, "InitialPassword");

        var initUser = user.Name;
        var initPassword = user.Password;
        var initSalt = user.Password;
        var initPwdCheck = authLogic.CheckPassword(user, "InitialPassword");

        authLogic.UpdatePassword(user, "NewPassword");
        var changedPwdCheck = authLogic.CheckPassword(user, "NewPassword");

        Assert.NotEmpty(initUser);
        Assert.NotEmpty(initPassword);
        Assert.NotEmpty(initSalt);
        Assert.True(initPwdCheck);
        Assert.NotEmpty(user.Password);
        Assert.NotEmpty(user.Salt);
        Assert.Equal(initUser, user.Name);
        Assert.NotEqual(initPassword, user.Password);
        Assert.NotEqual(initSalt, user.Salt);
        Assert.True(changedPwdCheck);
    }
}