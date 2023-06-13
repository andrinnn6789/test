using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class ToDbRoleMapper : ObjectMapper<Role, DataLayer.Model.Role>
{
    protected override DataLayer.Model.Role MapToDestination(Role source, DataLayer.Model.Role destination)
    {
        destination.Name = source.Name;

        return destination;
    }
}