using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class FromDbUserRoleMapper : ObjectMapper<DataLayer.Model.UserRole, UserRole>
{
    protected override UserRole MapToDestination(DataLayer.Model.UserRole source, UserRole destination)
    {
        destination.UserName = source.User.Name;
        destination.RoleName = source.Role.Name;

        return destination;
    }
}