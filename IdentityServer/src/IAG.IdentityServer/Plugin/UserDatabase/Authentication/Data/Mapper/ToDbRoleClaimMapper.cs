using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class ToDbRoleClaimMapper : ObjectMapper<RoleClaim, DataLayer.Model.RoleClaim>
{
    protected override DataLayer.Model.RoleClaim MapToDestination(RoleClaim source, DataLayer.Model.RoleClaim destination)
    {
        destination.AllowedPermissions = source.AllowedPermissions;

        return destination;
    }
}