using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;

public class ToDbUserClaimMapper : ObjectMapper<UserClaim, DataLayer.Model.UserClaim>
{
    protected override DataLayer.Model.UserClaim MapToDestination(UserClaim source, DataLayer.Model.UserClaim destination)
    {
        destination.AllowedPermissions = source.AllowedPermissions;

        return destination;
    }
}