using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Mapper;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Context;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Logic;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;
using IAG.Infrastructure.ObjectMapper;
using IAG.Infrastructure.Resource;
using IAG.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

using User = IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model.User;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication;

[PluginInfo("043AACA9-44D7-4AA8-916A-18527C7BBB9C", "IAG.IdentityServer.Plugin.UserDatabaseAuthentication")]
public class UserDatabaseAuthenticationPlugin : BaseAuthenticationPlugin<UserDatabaseAuthenticationConfig>
{
    private DbContextOptions<UserDbContext> _dbContextOptions;
    private readonly UserDatabaseAuthentication _authenticationLogic;
    public static readonly string RealmName = "Integrated";

    private DbContextOptions<UserDbContext> DbContextOptions =>
        _dbContextOptions ??= Env.EnvironmentName == "Test"
            ? new DbContextOptionsBuilder<UserDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options
            : new DbContextOptionsBuilder<UserDbContext>().UseSqlite(Config.ConnectionString).Options;

    public override string DefaultRealmName => RealmName;

    public UserDatabaseAuthenticationPlugin(IHostEnvironment environment) : base(environment)
    {
        _authenticationLogic = new UserDatabaseAuthentication();
    }

    public override void Init(IServiceProvider serviceProvider)
    {
        base.Init(serviceProvider);
        var userContext = new ExplicitUserContext(SchemaMigrator.MigratorUser, null);
        using var dbContext = new UserDbContext(DbContextOptions, userContext);
        var migrator = new SchemaMigrator(serviceProvider);
        migrator.DoMigration(dbContext);
    }

    public override IAuthenticationToken Authenticate(IRequestTokenParameter requestTokenParameter)
    {
        if (requestTokenParameter.GrantType != GrantTypes.Password)
        {
            throw new NotImplementedException($"Grant type '{requestTokenParameter.GrantType}' not supported");
        }

        var userContext = new ExplicitUserContext(requestTokenParameter.Username, requestTokenParameter.TenantId);
        using var dbContext = new UserDbContext(DbContextOptions, userContext);
        var dbUser = FindUser(requestTokenParameter.Username, requestTokenParameter.TenantId, dbContext);

        if (!_authenticationLogic.CheckPassword(dbUser, requestTokenParameter.Password))
        {
            throw new AuthenticationFailedException(ResourceIds.PasswordCheckFailed, requestTokenParameter.Username);
        }

        var token = new AuthenticationToken
        {
            Username = dbUser.Name,
            Email = dbUser.EMail,
            UserShouldChangePassword = dbUser.ChangePasswordAfterLogin,
            UserLanguage = dbUser.Culture
        };

        var requestedScopes = requestTokenParameter.Scopes?.ToHashSet() ?? new HashSet<string>();
        var grantedClaims = dbContext.GetGrantedClaims(dbUser, requestedScopes).ToList();
        if (grantedClaims.Count == 0 && requestedScopes.Count > 0)
        {
            throw new AuthorizationFailedException(ResourceIds.NoClaimsGranted);
        }

        token.Claims.AddRange(grantedClaims.Select(c => c.ToTokenClaim()));

        return token;
    }

    public override string GetEMail(string user, Guid? tenant)
    {
        var userContext = new ExplicitUserContext(user, tenant);
        using var dbContext = new UserDbContext(DbContextOptions, userContext);
        return FindUser(user, tenant, dbContext).EMail;
    }

    public override void ChangePassword(string user, Guid? tenant, string newPassword, bool changePasswordAfterLogin)
    {
        var userContext = new ExplicitUserContext(user, tenant);
        using var dbContext = new UserDbContext(DbContextOptions, userContext);
        var dbUser = FindUser(user, tenant, dbContext);
        dbUser.ChangePasswordAfterLogin = changePasswordAfterLogin;
        _authenticationLogic.UpdatePassword(dbUser, newPassword);
        dbContext.SaveChanges();
    }

    public void AddOrUpdateUser(Data.Model.User importUser, string roleName, string claimName, string scopeName, PermissionKind permission)
    {
        using var dbContext = new UserDbContext(DbContextOptions, new ExplicitUserContext("system", null));
        var role = dbContext.Roles.FirstOrDefault(r => r.Name == roleName);
        if (role == null)
        {
            role = new Role { Name = roleName };
            dbContext.Roles.Add(role);
        }

        var claim = dbContext.Claims.FirstOrDefault(r => r.Name.Equals(claimName) && r.Scope.Name.Equals(scopeName));
        if (claim == null)
            throw new NotFoundException($"Claim {claimName} for scope {scopeName}");

        var roleClaim = dbContext.RoleClaims.FirstOrDefault(r => r.Role == role && r.Claim == claim && r.AllowedPermissions == permission);
        if (roleClaim == null)
            dbContext.RoleClaims.Add(new RoleClaim { Role = role, Claim = claim, AllowedPermissions = permission });

        var dbUser = dbContext.Users.FirstOrDefault(r => r.Name == importUser.Name);
        if (dbUser == null)
        {
            dbUser = new ToDbUserMapper().NewDestination(importUser);
            if (!string.IsNullOrWhiteSpace(importUser.InitPassword))
            {
                SafePwd(importUser, dbUser);
            }

            dbContext.Users.Add(dbUser);
        }
        else
        {
            new ToDbUserMapper().UpdateDestination(dbUser, importUser);
        }

        var userRole = dbContext.UserRoles.FirstOrDefault(r => r.User == dbUser && r.Role == role);
        if (userRole == null)
            dbContext.UserRoles.Add(new UserRole { User = dbUser, Role = role });

        dbContext.SaveChanges();
    }

    private static User FindUser(string user, Guid? tenantId, UserDbContext dbContext)
    {
        var dbUser = dbContext.Users.FirstOrDefault(u => u.Name.ToLower() == user.ToLower() && u.TenantId == tenantId);
        if (dbUser == null)
            throw new AuthenticationFailedException(ResourceIds.UserNotFound, user);

        return dbUser;
    }

    public override void AddClaimDefinitions(IEnumerable<ClaimDefinition> claimDefinitions)
    {
        var userContext = new ExplicitUserContext("system", null);
        using var dbContext = new UserDbContext(DbContextOptions, userContext);
        foreach (var claimDefinition in claimDefinitions)
        {
            var scope = dbContext.Scopes.FirstOrDefault(s => s.Name == claimDefinition.ScopeName && s.TenantId == claimDefinition.TenantId);
            if (scope == null)
            {
                scope = new Scope
                {
                    Id = Guid.NewGuid(),
                    Name = claimDefinition.ScopeName,
                    TenantId = claimDefinition.TenantId
                };
                dbContext.Scopes.Add(scope);
            }
            scope.Disabled = false;

            var claim = dbContext.Claims.FirstOrDefault(c =>
                c.Name == claimDefinition.ClaimName &&
                c.TenantId == claimDefinition.TenantId &&
                c.ScopeId == scope.Id);
            if (claim == null)
            {
                claim = new Claim
                {
                    Id = Guid.NewGuid(),
                    Name = claimDefinition.ClaimName,
                    Scope = scope,
                    TenantId = claimDefinition.TenantId,
                    AvailablePermissions = claimDefinition.AvailablePermissions
                };
                dbContext.Claims.Add(claim);
            }
            else
            {
                claim.AvailablePermissions = claimDefinition.AvailablePermissions;
                claim.Disabled = false;
            }
        }

        dbContext.SaveChanges();
    }

    public override JObject GetExportData(IUserContext userContext)
    {
        var roleMapper = new FromDbRoleMapper();
        var roleClaimMapper = new FromDbRoleClaimMapper();
        var userMapper = new FromDbUserMapper();
        var userClaimMapper = new FromDbUserClaimMapper();
        var userRoleMapper = new FromDbUserRoleMapper();

        using var dbContext = new UserDbContext(DbContextOptions, userContext);
        var data = new UserDatabaseAuthenticationData()
        {
            Roles = dbContext.Roles.Where(r => !r.Disabled).Select(r => roleMapper.NewDestination(r)).ToList(),
            RoleClaims = dbContext.RoleClaims.Where(rc => !rc.Disabled).Include(rc => rc.Role).Include(rc => rc.Claim).ThenInclude(c => c.Scope).Select(rc => roleClaimMapper.NewDestination(rc)).ToList(),
            Users = dbContext.Users.Where(u => !u.Disabled).Select(u => userMapper.NewDestination(u)).ToList(),
            UserClaims = dbContext.UserClaims.Where(uc => !uc.Disabled).Include(uc => uc.User).Include(uc => uc.Claim).ThenInclude(c => c.Scope).Select(uc => userClaimMapper.NewDestination(uc)).ToList(),
            UserRoles = dbContext.UserRoles.Where(ur => !ur.Disabled).Include(ur => ur.User).Include(ur => ur.Role).Select(ur => userRoleMapper.NewDestination(ur)).ToList()
        };

        return JObject.FromObject(data);
    }

    public override void ImportData(JObject data, IUserContext userContext)
    {
        var toDbRoleMapper = new ToDbRoleMapper();
        var fromDbRoleMapper = new FromDbRoleMapper();
        var toDbUserMapper = new ToDbUserMapper();
        var fromDbUserMapper = new FromDbUserMapper();
        var toDbRoleClaimMapper = new ToDbRoleClaimMapper();
        var fromDbRoleClaimMapper = new FromDbRoleClaimMapper();
        var toDbUserClaimMapper = new ToDbUserClaimMapper();
        var fromDbUserClaimMapper = new FromDbUserClaimMapper();
        var toDbUserRoleMapper = new ToDbUserRoleMapper();
        var fromDbUserRoleMapper = new FromDbUserRoleMapper();

        using var dbContext = new UserDbContext(DbContextOptions, userContext);

        var newRoles = new List<Role>();
        foreach (var (full, patch) in GetListProperty<Data.Model.Role>(data, nameof(UserDatabaseAuthenticationData.Roles)))
        {
            var existingRole = dbContext.Roles
                .FirstOrDefault(dbRole => dbRole.Name == full.Name && dbRole.TenantId == null);
            if (existingRole == null)
            {
                newRoles.Add(toDbRoleMapper.NewDestination(full));
            }
            else
            {
                var patchedRole = fromDbRoleMapper.NewDestination(existingRole);
                ObjectPatcher.Patch(patchedRole, patch);
                toDbRoleMapper.UpdateDestination(existingRole, patchedRole);
            }
        }

        var newUsers = new List<User>();
        foreach (var (full, patch) in GetListProperty<Data.Model.User>(data, nameof(UserDatabaseAuthenticationData.Users)))
        {
            var existingUser = dbContext.Users
                .FirstOrDefault(dbUser => dbUser.Name == full.Name && dbUser.TenantId == null);
            if (existingUser == null)
            {
                var user = toDbUserMapper.NewDestination(full);
                newUsers.Add(user);
                SafePwd(full, user);
            }
            else
            {
                var patchedUser = fromDbUserMapper.NewDestination(existingUser);
                ObjectPatcher.Patch(patchedUser, patch);
                toDbUserMapper.UpdateDestination(existingUser, patchedUser);
            }
        }

        var newRoleClaims = new List<RoleClaim>();
        foreach (var (full, patch) in GetListProperty<Data.Model.RoleClaim>(data, nameof(UserDatabaseAuthenticationData.RoleClaims)))
        {
            var existingRoleClaim = dbContext.RoleClaims
                .Include(dbRoleClaim => dbRoleClaim.Role)
                .Include(dbRoleClaim => dbRoleClaim.Claim)
                .ThenInclude(dbClaim => dbClaim.Scope)
                .FirstOrDefault(dbRoleClaim =>
                    dbRoleClaim.Role.Name == full.RoleName && dbRoleClaim.Role.TenantId == null &&
                    dbRoleClaim.Claim.Name == full.ClaimName && dbRoleClaim.Claim.TenantId == null);

            if (existingRoleClaim == null)
            {
                var roleClaim = toDbRoleClaimMapper.NewDestination(full);
                roleClaim.Role = newRoles.FirstOrDefault(r => r.Name == full.RoleName) ??
                                 dbContext.Roles.First(r => r.Name == full.RoleName && r.TenantId == null);
                roleClaim.Claim = dbContext.Claims
                    .Include(c => c.Scope)
                    .First(c => c.Name == full.ClaimName && c.TenantId == null &&
                                c.Scope.Name == full.ScopeName && c.TenantId == null);

                newRoleClaims.Add(roleClaim);
            }
            else
            {
                var patchedRoleClaim = fromDbRoleClaimMapper.NewDestination(existingRoleClaim);
                ObjectPatcher.Patch(patchedRoleClaim, patch);
                toDbRoleClaimMapper.UpdateDestination(existingRoleClaim, patchedRoleClaim);
            }
        }

        var newUserClaims = new List<UserClaim>();
        foreach (var (full, patch) in GetListProperty<Data.Model.UserClaim>(data, nameof(UserDatabaseAuthenticationData.UserClaims)))
        {
            var existingUserClaim = dbContext.UserClaims
                .Include(dbRoleClaim => dbRoleClaim.User)
                .Include(dbRoleClaim => dbRoleClaim.Claim)
                .FirstOrDefault(dbRoleClaim =>
                    dbRoleClaim.User.Name == full.UserName && dbRoleClaim.User.TenantId == null &&
                    dbRoleClaim.Claim.Name == full.ClaimName && dbRoleClaim.Claim.TenantId == null);

            if (existingUserClaim == null)
            {
                var userClaim = toDbUserClaimMapper.NewDestination(full);
                userClaim.User = newUsers.FirstOrDefault(u => u.Name == full.UserName) ??
                                 dbContext.Users.First(u => u.Name == full.UserName && u.TenantId == null);
                userClaim.Claim = dbContext.Claims
                    .Include(c => c.Scope)
                    .First(c => c.Name == full.ClaimName && c.TenantId == null &&
                                c.Scope.Name == full.ScopeName && c.Scope.TenantId == null);

                newUserClaims.Add(userClaim);
            }
            else
            {
                var patchedUserClaim = fromDbUserClaimMapper.NewDestination(existingUserClaim);
                ObjectPatcher.Patch(patchedUserClaim, patch);
                toDbUserClaimMapper.UpdateDestination(existingUserClaim, patchedUserClaim);
            }
        }

        var newUserRoles = new List<UserRole>();
        foreach (var (full, patch) in GetListProperty<Data.Model.UserRole>(data, nameof(UserDatabaseAuthenticationData.UserRoles)))
        {
            var existingUserRole = dbContext.UserRoles
                .Include(dbRoleClaim => dbRoleClaim.User)
                .Include(dbRoleClaim => dbRoleClaim.Role)
                .FirstOrDefault(dbRoleClaim =>
                    dbRoleClaim.User.Name == full.UserName && dbRoleClaim.User.TenantId == null &&
                    dbRoleClaim.Role.Name == full.RoleName);

            if (existingUserRole == null)
            {
                var userRole = toDbUserRoleMapper.NewDestination(full);
                userRole.User = newUsers.FirstOrDefault(u => u.Name == full.UserName) ??
                                dbContext.Users.First(u => u.Name == full.UserName && u.TenantId == null);
                userRole.Role = newRoles.FirstOrDefault(r => r.Name == full.RoleName) ??
                                dbContext.Roles.First(r => r.Name == full.RoleName);

                newUserRoles.Add(userRole);
            }
            else
            {
                var patchedUserRole = fromDbUserRoleMapper.NewDestination(existingUserRole);
                ObjectPatcher.Patch(patchedUserRole, patch);
                toDbUserRoleMapper.UpdateDestination(existingUserRole, patchedUserRole);
            }
        }

        dbContext.Roles.AddRange(newRoles);
        dbContext.Users.AddRange(newUsers);
        dbContext.RoleClaims.AddRange(newRoleClaims);
        dbContext.UserClaims.AddRange(newUserClaims);
        dbContext.UserRoles.AddRange(newUserRoles);

        dbContext.SaveChanges();
    }

    private void SafePwd(Data.Model.User importUser, User dbUser)
    {
        if (importUser.InitPassword != null)
            _authenticationLogic.UpdatePassword(dbUser, importUser.InitPassword);
        File.AppendAllText(
            Path.Combine(new SettingsFinder().GetSettingsPath(), "UserDefaults.txt"),
            $"{importUser.Name}: {importUser.InitPassword}{Environment.NewLine}");
    }

    private static IEnumerable<(T Full, JObject Patch)> GetListProperty<T>(JObject data, string listName)
    {
        var list = data.Properties().FirstOrDefault(p => p.Name == listName)?.Value as JArray;

        return list?.Select(x => (x.ToObject<T>(), x as JObject)) ?? Enumerable.Empty<(T, JObject)>();
    }
}