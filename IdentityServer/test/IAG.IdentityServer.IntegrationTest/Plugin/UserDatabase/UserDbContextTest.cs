using System;
using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Context;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.IdentityServer.IntegrationTest.Plugin.UserDatabase;

public class UserDbContextTest
{
    [Fact]
    public void GrantedClaimsTest()
    {
        var user = new User
        {
            Name = "TestUser", 
            EMail = "TestUser@example.com"
        };
        var scopeA = new Scope
        {
            Id = Guid.NewGuid(),
            Name = "ScopeA"
        };
        var scopeB = new Scope
        {
            Id = Guid.NewGuid(),
            Name = "ScopeB"
        };
        var claimA = new Claim
        {
            Id = Guid.NewGuid(),
            Name = "ClaimA",
            Scope = scopeA,
            AvailablePermissions = PermissionKind.Read | PermissionKind.Update | PermissionKind.Delete
        };
        var claimB = new Claim
        {
            Id = Guid.NewGuid(),
            Name = "ClaimB",
            Scope = scopeA,
            AvailablePermissions = PermissionKind.Read | PermissionKind.Update | PermissionKind.Delete
        };
        var claimC = new Claim
        {
            Id = Guid.NewGuid(),
            Name = "ClaimC",
            Scope = scopeB,
            AvailablePermissions = PermissionKind.Read | PermissionKind.Update | PermissionKind.Delete
        };
        var claimD = new Claim
        {
            Id = Guid.NewGuid(),
            Name = "ClaimD",
            Scope = scopeB,
            AvailablePermissions = PermissionKind.Read | PermissionKind.Update | PermissionKind.Delete
        };
        var roleReadAAndC = new Role
        {
            Id = Guid.NewGuid(),
            RoleClaims = new List<RoleClaim>()
        };
        roleReadAAndC.RoleClaims.Add(new RoleClaim
            {
                Id = Guid.NewGuid(),
                Role = roleReadAAndC,
                Claim = claimA,
                AllowedPermissions = PermissionKind.Read
            }
        );
        roleReadAAndC.RoleClaims.Add(new RoleClaim
            {
                Id = Guid.NewGuid(),
                Role = roleReadAAndC,
                Claim = claimC,
                AllowedPermissions = PermissionKind.Read
            }
        );
        var userHasReadWriteClaimB = new UserClaim
        {
            Id = Guid.NewGuid(),
            User = user,
            Claim = claimB,
            AllowedPermissions = PermissionKind.Read | PermissionKind.Update
        };
        var userHasReadWriteClaimD = new UserClaim
        {
            Id = Guid.NewGuid(),
            User = user,
            Claim = claimD,
            AllowedPermissions = PermissionKind.Read | PermissionKind.Update
        };
        var userInRoleA = new UserRole
        {
            Id = Guid.NewGuid(),
            User = user,
            Role = roleReadAAndC
        };

        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        using var dbContext = new UserDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
        dbContext.Database.EnsureCreated();
        dbContext.Users.Add(user);
        dbContext.Scopes.Add(scopeA);
        dbContext.Scopes.Add(scopeB);
        dbContext.Claims.Add(claimA);
        dbContext.Claims.Add(claimB);
        dbContext.Claims.Add(claimC);
        dbContext.Claims.Add(claimD);
        dbContext.Roles.Add(roleReadAAndC);
        dbContext.UserClaims.Add(userHasReadWriteClaimB);
        dbContext.UserClaims.Add(userHasReadWriteClaimD);
        dbContext.UserRoles.Add(userInRoleA);
        dbContext.SaveChanges();

        var grantedClaimsScopeA = dbContext.GetGrantedClaims(user, new HashSet<string> { "ScopeA" }).ToList();
        var grantedClaimsScopeB = dbContext.GetGrantedClaims(user, new HashSet<string> { "ScopeB" }).ToList();
        var grantedClaimsScopeAAndB = dbContext.GetGrantedClaims(user, new HashSet<string> { "ScopeA", "ScopeB" }).ToList();
        var grantedClaimsAllScopes1 = dbContext.GetGrantedClaims(user, null).ToList();
        var grantedClaimsAllScopes2 = dbContext.GetGrantedClaims(user, new HashSet<string>()).ToList();
        var grantedClaimsUnknownScope = dbContext.GetGrantedClaims(user, new HashSet<string> { "UnknownScope"}).ToList();

        Assert.Equal(2, grantedClaimsScopeA.Count);
        Assert.All(grantedClaimsScopeA, c => Assert.Equal(scopeA.Name, c.ScopeName));
        Assert.Contains(grantedClaimsScopeA, c => c.ClaimName == claimA.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsScopeA, c => c.ClaimName == claimB.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));

        Assert.Equal(2, grantedClaimsScopeB.Count);
        Assert.All(grantedClaimsScopeB, c => Assert.Equal(scopeB.Name, c.ScopeName));
        Assert.Contains(grantedClaimsScopeB, c => c.ClaimName == claimC.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsScopeB, c => c.ClaimName == claimD.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));

        Assert.Equal(4, grantedClaimsScopeAAndB.Count);
        Assert.Contains(grantedClaimsScopeAAndB, c => c.ScopeName == scopeA.Name && c.ClaimName == claimA.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsScopeAAndB, c => c.ScopeName == scopeA.Name && c.ClaimName == claimB.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));
        Assert.Contains(grantedClaimsScopeAAndB, c => c.ScopeName == scopeB.Name && c.ClaimName == claimC.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsScopeAAndB, c => c.ScopeName == scopeB.Name && c.ClaimName == claimD.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));

        Assert.Equal(4, grantedClaimsAllScopes1.Count);
        Assert.Contains(grantedClaimsAllScopes1, c => c.ScopeName == scopeA.Name && c.ClaimName == claimA.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsAllScopes1, c => c.ScopeName == scopeA.Name && c.ClaimName == claimB.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));
        Assert.Contains(grantedClaimsAllScopes1, c => c.ScopeName == scopeB.Name && c.ClaimName == claimC.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsAllScopes1, c => c.ScopeName == scopeB.Name && c.ClaimName == claimD.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));

        Assert.Equal(4, grantedClaimsAllScopes2.Count);
        Assert.Contains(grantedClaimsAllScopes2, c => c.ScopeName == scopeA.Name && c.ClaimName == claimA.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsAllScopes2, c => c.ScopeName == scopeA.Name && c.ClaimName == claimB.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));
        Assert.Contains(grantedClaimsAllScopes2, c => c.ScopeName == scopeB.Name && c.ClaimName == claimC.Name && c.AllowedPermissions == PermissionKind.Read);
        Assert.Contains(grantedClaimsAllScopes2, c => c.ScopeName == scopeB.Name && c.ClaimName == claimD.Name && c.AllowedPermissions == (PermissionKind.Read | PermissionKind.Update));

        Assert.Empty(grantedClaimsUnknownScope);

        dbContext.Database.EnsureDeleted();
    }
}