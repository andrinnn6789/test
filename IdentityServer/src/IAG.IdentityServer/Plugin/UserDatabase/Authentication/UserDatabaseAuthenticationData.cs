using System.Collections.Generic;

using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication;

public class UserDatabaseAuthenticationData
{
    public List<Role> Roles { get; set; }

    public List<RoleClaim> RoleClaims { get; set; }

    public List<User> Users { get; set; }

    public List<UserRole> UserRoles { get; set; }

    public List<UserClaim> UserClaims { get; set; }
}