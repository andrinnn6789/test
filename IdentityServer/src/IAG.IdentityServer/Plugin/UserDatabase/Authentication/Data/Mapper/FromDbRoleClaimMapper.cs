using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class FromDbRoleClaimMapper : ObjectMapper<DataLayer.Model.RoleClaim, RoleClaim>
{
    protected override RoleClaim MapToDestination(DataLayer.Model.RoleClaim source, RoleClaim destination)
    {
        destination.RoleName = source.Role.Name;
        destination.ScopeName = source.Claim.Scope.Name;
        destination.ClaimName = source.Claim.Name;
        destination.AllowedPermissions = source.AllowedPermissions;

        return destination;
    }
}