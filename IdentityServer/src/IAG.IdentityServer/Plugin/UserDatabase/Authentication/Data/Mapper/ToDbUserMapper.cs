
using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Logic;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class ToDbUserMapper : ObjectMapper<User, DataLayer.Model.User>
{
    protected override DataLayer.Model.User MapToDestination(User source, DataLayer.Model.User destination)
    {
        destination.Name = source.Name;
        destination.ChangePasswordAfterLogin = source.ChangePasswordAfterLogin;
        destination.EMail = source.EMail;
        destination.FirstName = source.FirstName;
        destination.LastName = source.LastName;
        destination.Culture = source.Culture;

        if (!string.IsNullOrEmpty(source.Password))
        {
            new UserDatabaseAuthentication().UpdatePassword(destination, source.Password);
        }

        return destination;
    }
}