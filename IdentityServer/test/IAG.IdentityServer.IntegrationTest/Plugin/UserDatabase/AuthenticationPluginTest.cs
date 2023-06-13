using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;

using IAG.IdentityServer.Plugin.UserDatabase.Authentication;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Context;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Logic;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Newtonsoft.Json.Linq;

using Xunit;

using Claim = IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model.Claim;

namespace IAG.IdentityServer.IntegrationTest.Plugin.UserDatabase;

public sealed class AuthenticationPluginTest : IDisposable
{
    private const string TestUser = "TestUser";
    private const string TestUserLowercase = "testuser";
    private static readonly Guid TestTenantA = Guid.NewGuid();
    private static readonly Guid TestTenantB = Guid.NewGuid();
    private const string TestPassword = "TestPassword";

    private readonly UserDatabaseAuthenticationPlugin _plugin;
    private readonly UserDbContext _dbContext;
    private readonly User _user1A;

    public AuthenticationPluginTest()
    {
        _plugin = new UserDatabaseAuthenticationPlugin(new Mock<IHostEnvironment>().Object)
        {
            Config = {ConnectionString = $"Data Source={Path.GetTempFileName()}"}
        };

        var tenantA = new Tenant { Id = TestTenantA, Name = "TenantA" };
        var tenantB = new Tenant { Id = TestTenantB, Name = "TenantB"};
        _user1A = new User { Name = TestUser, EMail = "TestUser@example.com", Tenant = tenantA, Culture = "de"};
        new UserDatabaseAuthentication().UpdatePassword(_user1A, TestPassword);
        var user1B = new User { Name = TestUser, EMail = "TestUser@example.com", Tenant = tenantB };
        new UserDatabaseAuthentication().UpdatePassword(user1B, TestPassword);

        _dbContext = new UserDbContext(new DbContextOptionsBuilder<UserDbContext>().UseSqlite(_plugin.Config.ConnectionString).Options, new ExplicitUserContext(TestUser, null));
        _dbContext.Database.EnsureCreated();
        _dbContext.Tenants.Add(tenantA);
        _dbContext.Tenants.Add(tenantB);
        _dbContext.Users.Add(_user1A);
        _dbContext.Users.Add(user1B);
        _dbContext.SaveChanges();
    }

    [Fact]
    public void ConstructorTest()
    {
        Assert.NotEqual(Guid.Empty, _plugin.PluginId);
        Assert.NotEmpty(_plugin.PluginName);
        Assert.NotEmpty(_plugin.DefaultRealmName);
    }

    [Fact]
    public void InitTest()
    {
        var plugin = new UserDatabaseAuthenticationPlugin(new Mock<IHostEnvironment>().Object)
        {
            Config = {ConnectionString = $"Data Source={Path.GetTempFileName()}"}
        };

        plugin.Init(new ServiceCollection().BuildServiceProvider());

        using var initializedDbContext = new UserDbContext(new DbContextOptionsBuilder<UserDbContext>().UseSqlite(plugin.Config.ConnectionString).Options, new ExplicitUserContext(TestUser, null));
        var users = initializedDbContext.Users.ToList();
        var scopes = initializedDbContext.Scopes.ToList();
        var claims = initializedDbContext.Claims.ToList();
        var roles = initializedDbContext.Roles.ToList();

        initializedDbContext.Database.EnsureDeleted();

        Assert.Empty(users);
        Assert.Empty(scopes);
        Assert.Empty(claims);
        Assert.Empty(roles);
    }

    [Fact]
    public void AuthenticationOkTest()
    {
        var scope = new Scope
        {
            Name = "Scope"
        };
        var claimA = new Claim
        {
            Name = "ClaimA",
            Scope = scope,
            AvailablePermissions = PermissionKind.Read | PermissionKind.Update | PermissionKind.Delete
        };
        var claimB = new Claim
        {
            Name = "ClaimB",
            Scope = scope,
            AvailablePermissions = PermissionKind.Read | PermissionKind.Update | PermissionKind.Delete
        };
        var roleReadA = new Role
        {
            RoleClaims = new List<RoleClaim>()
        };
        roleReadA.RoleClaims.Add(new RoleClaim
            {
                Role = roleReadA,
                Claim = claimA,
                AllowedPermissions = PermissionKind.Read
            }
        );
        var userHasReadWriteClaimB = new UserClaim
        {
            User = _user1A,
            Claim = claimB,
            AllowedPermissions = PermissionKind.Read | PermissionKind.Update
        };
        var userInRoleA = new UserRole
        {
            User = _user1A,
            Role = roleReadA
        };
        _dbContext.Scopes.Add(scope);
        _dbContext.Claims.Add(claimA);
        _dbContext.Claims.Add(claimB);
        _dbContext.Roles.Add(roleReadA);
        _dbContext.UserClaims.Add(userHasReadWriteClaimB);
        _dbContext.UserRoles.Add(userInRoleA);
        _dbContext.SaveChanges();

        var tokenA = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, TestPassword, TestTenantA));
        var tokenB = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, TestPassword, TestTenantB));

        Assert.NotNull(tokenA);
        Assert.NotNull(tokenA.Claims);
        Assert.Contains(tokenA.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Read|Update:ClaimB@Scope");
        Assert.Contains(tokenA.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Read:ClaimA@Scope");
        Assert.Equal(2, tokenA.Claims.Count);
        Assert.Equal("de", tokenA.UserLanguage);
        Assert.NotNull(tokenB);
        Assert.NotNull(tokenB.Claims);
        Assert.Empty(tokenB.Claims);
    }

    [Fact]
    public void AuthenticationFailTest()
    {
        var tokenRequest = RequestTokenParameter.ForPasswordGrant(TestUser, TestPassword, TestTenantA);
        tokenRequest.ScopeSetter = "xx";

        Assert.Throws<NotImplementedException>(() => _plugin.Authenticate(new RequestTokenParameter { GrantType = GrantTypes.ClientCredentials }));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, $"not_{TestPassword}", TestTenantA)));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant("UnknownUser", "Irrelevant", TestTenantA)));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, TestPassword)));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUserLowercase, TestPassword)));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, TestPassword, Guid.NewGuid())));
        Assert.Throws<AuthorizationFailedException>(() => _plugin.Authenticate(tokenRequest));
    }

    [Fact]
    public void CorrectLanguageTest()
    {
        Assert.Throws<CultureNotFoundException>(() => new User { Name = "x", Culture = "x" });
    }

    [Fact]
    public void GetEMailTest()
    {
        var email = _plugin.GetEMail(TestUser, TestTenantA);
        var emailLowercase = _plugin.GetEMail(TestUserLowercase, TestTenantA);

        Assert.NotEmpty(email);
        Assert.NotEmpty(emailLowercase);
        Assert.Throws<AuthenticationFailedException>(() => _plugin.GetEMail("UnknownUser", TestTenantA));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.GetEMail(TestUser, null));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.GetEMail(TestUserLowercase, null));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.GetEMail(TestUser, Guid.NewGuid()));
    }

    [Fact]
    public void ChangePasswordTest()
    {
        var newPassword = "newPassword";
        _plugin.ChangePassword(TestUser, TestTenantA, newPassword, false);
        var tokenUserA = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, newPassword, TestTenantA));
        var tokenUserALower = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUserLowercase, newPassword, TestTenantA));
        var tokenUserB = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(TestUser, TestPassword, TestTenantB));
        _plugin.ChangePassword(TestUser, TestTenantA, TestPassword, false);

        Assert.NotNull(tokenUserA);
        Assert.NotNull(tokenUserALower);
        Assert.NotNull(tokenUserB);
        Assert.Throws<AuthenticationFailedException>(() => _plugin.ChangePassword("UnknownUser", TestTenantA, "Irrelevant", false));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.ChangePassword(TestUser, null, "Irrelevant", false));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.ChangePassword(TestUser, Guid.NewGuid(), "Irrelevant", false));
    }

    [Fact]
    public void AddClaimDefinitionsTest()
    {
        var scope1 = new Scope
        {
            Name = "Scope1"
        };
        var claimA = new Claim
        {
            Name = "ClaimA",
            Scope = scope1,
            AvailablePermissions = PermissionKind.Read
        };

        _dbContext.Scopes.Add(scope1);
        _dbContext.Claims.Add(claimA);
        _dbContext.SaveChanges();

        var claimDefinitions = new List<ClaimDefinition>
        {
            new() { ScopeName = "Scope1", ClaimName = "ClaimA", AvailablePermissions = PermissionKind.Read | PermissionKind.Update },
            new() { ScopeName = "Scope2", ClaimName = "ClaimA", AvailablePermissions = PermissionKind.Read },
            new() { ScopeName = "Scope1", ClaimName = "ClaimB", AvailablePermissions = PermissionKind.Read }
        };

        _plugin.AddClaimDefinitions(claimDefinitions);

        _dbContext.Entry(claimA).State = EntityState.Detached;  // ensure reload of entity!
        var claimAUpdated = _dbContext.Claims.FirstOrDefault(c => c.Id == claimA.Id);
        var scope2 = _dbContext.Scopes.FirstOrDefault(s => s.Name == "Scope2");
        Assert.NotNull(claimAUpdated);
        Assert.NotNull(scope2);
        Assert.Equal(PermissionKind.Read | PermissionKind.Update, claimAUpdated.AvailablePermissions);
        Assert.Single(_dbContext.Claims, s => s.Name == "ClaimA" && s.ScopeId == scope2.Id);
        Assert.Single(_dbContext.Claims, s => s.Name == "ClaimB" && s.ScopeId == scope1.Id);
    }

    [Fact]
    public void AddOrUpdateUserInitPassword()
    {
        var scope1 = new Scope
        {
            Name = "Scope1"
        };
        var claimA = new Claim
        {
            Name = "ClaimA",
            Scope = scope1,
            AvailablePermissions = PermissionKind.Read
        };

        _dbContext.Scopes.Add(scope1);
        _dbContext.Claims.Add(claimA);
        _dbContext.SaveChanges();

        _plugin.AddOrUpdateUser(
            new IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User
                { Name = "test", InitPassword = "test" },
            "test", "ClaimA", "Scope1", PermissionKind.Read);

        Assert.NotNull(_plugin.Authenticate(RequestTokenParameter.ForPasswordGrant("test", "test")));

        _plugin.AddOrUpdateUser(
            new IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User
                { Name = "test", InitPassword = "init" },
            "init", "ClaimA", "Scope1", PermissionKind.Read);

        Assert.NotNull(_plugin.Authenticate(RequestTokenParameter.ForPasswordGrant("test", "test")));
        Assert.Throws<AuthenticationFailedException>(() => _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant("test", "init")));
    }

    [Fact]
    public void AddOrUpdateUserTest()
    {
        var scope1 = new Scope
        {
            Name = "Scope1"
        };
        var claimA = new Claim
        {
            Name = "ClaimA",
            Scope = scope1,
            AvailablePermissions = PermissionKind.Read
        };

        _dbContext.Scopes.Add(scope1);
        _dbContext.Claims.Add(claimA);
        _dbContext.SaveChanges();

        _plugin.AddOrUpdateUser(
            new IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User
                { Name = "test", Password = "test"}, 
            "test", "ClaimA", "Scope1", PermissionKind.Read);
        _plugin.AddOrUpdateUser(
            new IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User
                { Name = "test", Password = "test", Culture = "de"}, 
            "test", "ClaimA", "Scope1", PermissionKind.Read);

        Assert.Single(_dbContext.Users, s => s.Name == "test");
        Assert.Single(_dbContext.Roles, s => s.Name == "test");
        Assert.Single(_dbContext.UserRoles, s => s.User.Name == "test" && s.Role.Name == "test");
        Assert.Single(_dbContext.RoleClaims, s => s.Claim.Name == "ClaimA" && s.Role.Name == "test" && s.AllowedPermissions == PermissionKind.Read);
        Assert.NotNull(_plugin.Authenticate(RequestTokenParameter.ForPasswordGrant("test", "test")));
        Assert.Throws<NotFoundException>(
            () => _plugin.AddOrUpdateUser(new IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User
                    { Name = "test", Password = "test", Culture = "de" }, 
                "test", "xxx", "Scope1", PermissionKind.Read));

        _plugin.AddOrUpdateUser(new IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User
                { Name = "test2", Password = "test", Culture = "de" }, 
            "test", "ClaimA", "Scope1", PermissionKind.Read);
        Assert.Single(_dbContext.Roles, s => s.Name == "test");
        Assert.Single(_dbContext.RoleClaims, s => s.Claim.Name == "ClaimA" && s.Role.Name == "test" && s.AllowedPermissions == PermissionKind.Read);
    }

    [Fact]
    public void GetExportDataTest()
    {
        var scope = new Scope {Name = "Scope"};
        var claim = new Claim {Name = "Claim", Scope = scope};
        var role = new Role { RoleClaims = new List<RoleClaim>() };
        var userClaim = new UserClaim {User = _dbContext.Users.First(),Claim = claim};
        var userRole = new UserRole {User = _dbContext.Users.First(), Role = role};
        role.RoleClaims.Add(new RoleClaim { Role = role, Claim = claim });

        _dbContext.Scopes.Add(scope);
        _dbContext.Claims.Add(claim);
        _dbContext.Roles.Add(role);
        _dbContext.UserClaims.Add(userClaim);
        _dbContext.UserRoles.Add(userRole);
        _dbContext.SaveChanges();

        var rawExportData = _plugin.GetExportData(new ExplicitUserContext("Test", null));
        var exportData = rawExportData.ToObject<UserDatabaseAuthenticationData>();

        Assert.NotNull(exportData);
        Assert.Single(exportData.Roles);
        Assert.Single(exportData.RoleClaims);
        Assert.Single(exportData.UserRoles);
        Assert.Single(exportData.UserClaims);
        Assert.Equal(2, exportData.Users.Count);
    }

    [Fact]
    public void ImportTest()
    {
        var scope = new Scope {Name = "Scope"};
        var claim = new Claim {Name = "Claim", Scope = scope};

        _dbContext.Scopes.Add(scope);
        _dbContext.Claims.Add(claim);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.SaveChanges();

        var importData = new UserDatabaseAuthenticationData
        {
            Users = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User>
            {
                new()
                {
                    Name = "ImportedUser",
                    Password = "ImportedPassword"
                },
                new()
                {
                    Name = "ImportedUser2",
                    Password = "ImportedPassword2",
                    LastName = "de"
                }
            },
            Roles = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.Role>
            {
                new()
                {
                    Name = "ImportedRole"
                }
            },
            RoleClaims = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.RoleClaim>
            {
                new()
                {
                    ScopeName = scope.Name,
                    ClaimName = claim.Name,
                    RoleName = "ImportedRole",
                    AllowedPermissions = PermissionKind.Crud
                }
            },
            UserRoles = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.UserRole>
            {
                new()
                {
                    UserName = "ImportedUser",
                    RoleName = "ImportedRole"
                }
            },
            UserClaims = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.UserClaim>
            {
                new()
                {
                    UserName = "ImportedUser",
                    ScopeName = scope.Name,
                    ClaimName = claim.Name,
                    AllowedPermissions = PermissionKind.Crud
                }
            }
        };

        _plugin.ImportData(JObject.FromObject(importData), new ExplicitUserContext(TestUser, TestTenantA));
        var token = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant("ImportedUser", "ImportedPassword"));

        using var initializedDbContext =
            new UserDbContext(new DbContextOptionsBuilder<UserDbContext>().UseSqlite(_plugin.Config.ConnectionString).Options,
                new ExplicitUserContext(TestUser, null));
        var users = initializedDbContext.Users.ToList();
        var roles = initializedDbContext.Roles.ToList();
        var roleClaims = initializedDbContext.RoleClaims.Include(rc => rc.Role).ToList();
        var userClaims = initializedDbContext.UserClaims.Include(uc => uc.User).ToList();
        var userRoles = initializedDbContext.UserRoles.Include(uc => uc.User).Include(uc => uc.Role).ToList();

        initializedDbContext.Database.EnsureDeleted();

        Assert.Equal(2, users.Count);
        var importedUser = users.First(u => u.Name.Equals("ImportedUser"));
        var importedRole = Assert.Single(roles);
        var importedRoleClaim = Assert.Single(roleClaims);
        var importedUserClaim = Assert.Single(userClaims);
        var importedUserRole = Assert.Single(userRoles);
        Assert.NotNull(importedRole);
        Assert.NotNull(importedRoleClaim);
        Assert.NotNull(importedUserClaim);
        Assert.NotNull(importedUserRole);
        Assert.Equal("ImportedUser", importedUser.Name);
        Assert.Equal("ImportedRole", importedRole.Name);
        Assert.Equal("ImportedRole", importedRoleClaim.Role?.Name);
        Assert.Equal("ImportedUser", importedUserRole.User?.Name);
        Assert.Equal("ImportedRole", importedUserRole.Role?.Name);
        Assert.NotEmpty(importedUser.Password);
        Assert.NotNull(token);
    }

    [Fact]
    public void ImportUpdateTest()
    {
        var scope = new Scope { Name = "Scope" };
        var claimForRole = new Claim { Name = "ClaimForRole", Scope = scope };
        var claimForUser = new Claim { Name = "ClaimForUser", Scope = scope };
        var role = new Role() { Name = "Role" };
        var user = new User { Name = "User", EMail = "EMail" };
        var user2 = new User { Name = "User2" };
        var roleClaim = new RoleClaim() { Role = role, Claim = claimForRole, AllowedPermissions = PermissionKind.Read };
        var userRole = new UserRole() { User = user, Role = role };
        var userClaim = new UserClaim() { User = user, Claim = claimForUser, AllowedPermissions = PermissionKind.Read };

        new UserDatabaseAuthentication().UpdatePassword(user, "TestPassword");
        new UserDatabaseAuthentication().UpdatePassword(user2, "TestPassword");

        _dbContext.Scopes.Add(scope);
        _dbContext.Claims.Add(claimForRole);
        _dbContext.Claims.Add(claimForUser);
        _dbContext.Roles.Add(role);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Users.Add(user);
        _dbContext.Users.Add(user2);
        _dbContext.RoleClaims.Add(roleClaim);
        _dbContext.UserRoles.Add(userRole);
        _dbContext.UserClaims.Add(userClaim);

        _dbContext.SaveChanges();

        var importData = new UserDatabaseAuthenticationData
        {
            Users = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User>
            {
                new()
                {
                    Name = user.Name,
                    EMail = "UpdatedEMail",
                    LastName = "UpdatedLastName"
                },
                new()
                {
                    Name = user2.Name,
                    Password = "UpdatedPassword"
                }
            },
            Roles = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.Role>
            {
                new()
                {
                    Name = role.Name
                }
            },
            RoleClaims = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.RoleClaim>
            {
                new()
                {
                    ScopeName = scope.Name,
                    ClaimName = claimForRole.Name,
                    RoleName = role.Name,
                    AllowedPermissions = PermissionKind.All
                }
            },
            UserRoles = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.UserRole>
            {
                new()
                {
                    UserName = user.Name,
                    RoleName = role.Name
                }
            },
            UserClaims = new List<IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.UserClaim>
            {
                new()
                {
                    UserName = user.Name,
                    ScopeName = scope.Name,
                    ClaimName = claimForUser.Name,
                    AllowedPermissions = PermissionKind.All
                }
            }
        };

        _plugin.ImportData(JObject.FromObject(importData), new ExplicitUserContext(TestUser, TestTenantA));
        var token = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(user.Name, "TestPassword"));
        var token2 = _plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(user2.Name, "UpdatedPassword"));

        using var initializedDbContext =
            new UserDbContext(new DbContextOptionsBuilder<UserDbContext>().UseSqlite(_plugin.Config.ConnectionString).Options,
                new ExplicitUserContext(TestUser, null));
        var users = initializedDbContext.Users.ToList();
        var roles = initializedDbContext.Roles.ToList();
        var roleClaims = initializedDbContext.RoleClaims.Include(rc => rc.Role).Include(rc => rc.Claim).ToList();
        var userClaims = initializedDbContext.UserClaims.Include(uc => uc.User).Include(uc => uc.Claim).ToList();
        var userRoles = initializedDbContext.UserRoles.Include(uc => uc.User).Include(uc => uc.Role).ToList();

        initializedDbContext.Database.EnsureDeleted();

        Assert.Equal(2, users.Count);
        var updatedUser = Assert.Single(users, u => u.Name == user.Name);
        var updatedUser2 = Assert.Single(users, u => u.Name == user2.Name);
        var updatedRole = Assert.Single(roles);
        var updatedRoleClaim = Assert.Single(roleClaims);
        var updatedUserClaim = Assert.Single(userClaims);
        var updatedUserRole = Assert.Single(userRoles);
        Assert.NotNull(updatedUser);
        Assert.NotNull(updatedUser2);
        Assert.NotNull(updatedRole);
        Assert.NotNull(updatedRoleClaim);
        Assert.NotNull(updatedUserClaim);
        Assert.NotNull(updatedUserRole);
        Assert.Equal("UpdatedEMail", updatedUser.EMail);
        Assert.Equal("UpdatedLastName", updatedUser.LastName);
        Assert.Equal(role.Name, updatedRole.Name);
        Assert.Equal(role.Name, updatedRoleClaim.Role?.Name);
        Assert.Equal(claimForRole.Name, updatedRoleClaim.Claim?.Name);
        Assert.Equal(PermissionKind.All, updatedRoleClaim.AllowedPermissions);
        Assert.Equal(user.Name, updatedUserClaim.User?.Name);
        Assert.Equal(claimForUser.Name, updatedUserClaim.Claim?.Name);
        Assert.Equal(PermissionKind.All, updatedUserClaim.AllowedPermissions);
        Assert.Equal(user.Name, updatedUserRole.User?.Name);
        Assert.Equal(role.Name, updatedUserRole.Role?.Name);
        Assert.NotNull(token);
        Assert.NotNull(token2);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}