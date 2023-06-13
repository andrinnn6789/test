using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Authorization.Model;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;
using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Context;

[ContextInfo("UserDb")]
public class UserDbContext : BaseDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options, IUserContext userContext) : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<User> Users { get; set; }
    [UsedImplicitly]
    public DbSet<Scope> Scopes { get; set; }
    [UsedImplicitly]
    public DbSet<Claim> Claims { get; set; }
    [UsedImplicitly]
    public DbSet<Role> Roles { get; set; }
    [UsedImplicitly]
    public DbSet<RoleClaim> RoleClaims { get; set; }
    [UsedImplicitly]
    public DbSet<UserRole> UserRoles { get; set; }
    [UsedImplicitly]
    public DbSet<UserClaim> UserClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Claim>()
            .HasIndex(nameof(Claim.ScopeId), nameof(Claim.Name)).IsUnique();
    }

    public IEnumerable<GrantedClaim> GetGrantedClaims(User user, HashSet<string> requestedScopes)
    {
        var scopes = Scopes.AsNoTracking();
        if (requestedScopes?.Any() == true)
        {
            scopes = scopes.Where(s => requestedScopes.Contains(s.Name));
        }

        var userClaims = from userClaim in UserClaims.AsNoTracking().Where(uc => uc.UserId == user.Id)
            join claim in Claims.AsNoTracking() on userClaim.ClaimId equals claim.Id
            join scope in scopes on claim.ScopeId equals scope.Id
            select new GrantedClaim
            {
                ScopeName = scope.Name,
                ClaimName = claim.Name,
                AllowedPermissions = userClaim.AllowedPermissions
            };


        var userRoleClaims = from userRole in UserRoles.AsNoTracking().Where(uc => uc.UserId == user.Id)
            join roleClaim in RoleClaims.AsNoTracking() on userRole.RoleId equals roleClaim.RoleId
            join claim in Claims.AsNoTracking() on roleClaim.ClaimId equals claim.Id
            join scope in scopes on claim.ScopeId equals scope.Id
            select new GrantedClaim
            {
                ScopeName = scope.Name,
                ClaimName = claim.Name,
                AllowedPermissions = roleClaim.AllowedPermissions
            };

        return userClaims.Concat(userRoleClaims);
    }
}