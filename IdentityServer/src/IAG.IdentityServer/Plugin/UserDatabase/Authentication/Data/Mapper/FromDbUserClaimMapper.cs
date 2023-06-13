using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class FromDbUserClaimMapper : ObjectMapper<DataLayer.Model.UserClaim, UserClaim>
{
    protected override UserClaim MapToDestination(DataLayer.Model.UserClaim source, UserClaim destination)
    {
        destination.UserName = source.User.Name;
        destination.ScopeName = source.Claim.Scope.Name;
        destination.ClaimName = source.Claim.Name;
        destination.AllowedPermissions = source.AllowedPermissions;

        return destination;
    }
}