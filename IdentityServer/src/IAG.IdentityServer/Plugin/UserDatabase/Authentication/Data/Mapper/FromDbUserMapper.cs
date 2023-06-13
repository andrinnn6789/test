using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class FromDbUserMapper : ObjectMapper<DataLayer.Model.User, User>
{
    protected override User MapToDestination(DataLayer.Model.User source, User destination)
    {
        destination.Name = source.Name;
        destination.ChangePasswordAfterLogin = source.ChangePasswordAfterLogin;
        destination.EMail = source.EMail;
        destination.FirstName = source.FirstName;
        destination.LastName = source.LastName;
        destination.Password = string.Empty;
        destination.Culture = source.Culture;

        return destination;
    }
}