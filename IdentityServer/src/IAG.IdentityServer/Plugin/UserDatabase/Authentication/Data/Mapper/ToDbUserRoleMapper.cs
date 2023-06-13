using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class ToDbUserRoleMapper : ObjectMapper<UserRole, DataLayer.Model.UserRole>
{
    protected override DataLayer.Model.UserRole MapToDestination(UserRole source, DataLayer.Model.UserRole destination)
    {
        return destination;
    }
}