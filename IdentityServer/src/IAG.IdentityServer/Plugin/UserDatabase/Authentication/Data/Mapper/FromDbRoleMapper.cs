using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class FromDbRoleMapper : ObjectMapper<DataLayer.Model.Role, Role>
{
    protected override Role MapToDestination(DataLayer.Model.Role source, Role destination)
    {
        destination.Name = source.Name;

        return destination;
    }
}