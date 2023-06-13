using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.ControlCenter.Mobile.BusinessLayer;
using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Context;
using IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using Moq;

using Xunit;

using User = IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model.User;

namespace IAG.ControlCenter.Test.Mobile.BusinessLayer;

public class MobileLicenceAdminTest : IClassFixture<AuthPluginFixture>, IDisposable
{
    private readonly TestContextBuilder _builder;
    private readonly AuthPluginFixture _authPlugin;

    public MobileLicenceAdminTest(AuthPluginFixture fixture)
    {
        _builder = new TestContextBuilder();
        _authPlugin = fixture;
    }

    public void Dispose()
    {
        _builder.Dispose();
    }

    [Fact]
    public void CheckAllIdentical()
    {
        var localSystems = BuildMobileLicenceSync();
        var response = new LicenceAdmin(_builder.Context, _authPlugin.AuthPlugin).SyncSystems(localSystems);
        Assert.Empty(response);
    }

    [Fact]
    public void CheckRemoveLicAndSystem()
    {
        var localSystems = BuildMobileLicenceSync();
        localSystems.Installations.Remove(localSystems.Installations.First(s => s.Id.Equals(_builder.Installation2.Id)));
        localSystems.Licences.Remove(localSystems.Licences.First(l => l.Licence.Equals("new")));
        localSystems.Modules.Remove(localSystems.Modules.First(m => m.Licence.Equals("new")));

        _builder.Context.SaveChanges();  // ensure no tracking
        Assert.Equal(2, _builder.Context.MobileInstallation.Count());
        Assert.Equal(3, _builder.Context.MobileLicences.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(7, _builder.Context.MobileModules.Count());
        Assert.Equal(5, _builder.Context.MobileModules.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(2, _builder.Context.MobileModules.Count(l => l.Tenant.Equals(_builder.Tenant2)));

        var response = new LicenceAdmin(_builder.Context, _authPlugin.AuthPlugin).SyncSystems(localSystems);
        Assert.Empty(response);
        Assert.Equal(1, _builder.Context.MobileInstallation.Count());
        Assert.Equal(2, _builder.Context.MobileLicences.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(6, _builder.Context.MobileModules.Count());
        Assert.Equal(4, _builder.Context.MobileModules.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(2, _builder.Context.MobileModules.Count(l => l.Tenant.Equals(_builder.Tenant2)));
    }

    [Fact]
    public void CheckAddLicAndSystem()
    {
        var localSystems = BuildMobileLicenceSync();
        localSystems.Tenants.First(t => t.Name.Equals("Tenant2")).Name = "Tenant2_Upd";
        var tenant3 = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Tenant3"
        };
        localSystems.Tenants.Add(tenant3);
        var addedInstallation = new MobileInstallation
        {
            Id = Guid.NewGuid(),
            Url = "Added3",
            Tenant = tenant3,
            Color ="#000000"
        };
        localSystems.Installations.Add(addedInstallation);
        localSystems.Licences.AddRange(new List<MobileLicence>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Licence = "AddedLic1", 
                LicenceStatus = LicenceStatusEnum.New, 
                Tenant = tenant3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Licence = "AddedLic2", 
                LicenceStatus = LicenceStatusEnum.New, 
                Tenant = tenant3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Licence = "AddedTo1Lic", 
                LicenceStatus = LicenceStatusEnum.New, 
                Tenant = _builder.Tenant1
            }
        });
        localSystems.Modules.AddRange(new List<MobileModule>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Licence = "AddedLic1", 
                ModuleName = "ArticleInformation",
                LicencedUntil = DateTime.Today.AddYears(1),
                Tenant = tenant3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Licence = "AddedLic2", 
                ModuleName = "Inventory",
                LicencedUntil = DateTime.Today.AddYears(1),
                Tenant = tenant3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Licence = "AddedTo1Lic", 
                ModuleName = "ArticleInformation",
                LicencedUntil = DateTime.Today.AddYears(1),
                Tenant = _builder.Tenant1
            }
        });
            
        localSystems.Licences.First(l => l.Licence.Equals("new")).DeviceInfo = "changed";
        localSystems.Licences.First(l => l.Licence.Equals("inUse1")).LicenceStatus = LicenceStatusEnum.Reset;

        _builder.Context.SaveChanges();  // ensure no tracking
        Assert.Equal(2, _builder.Context.Tenants.Count());
        Assert.Equal(2, _builder.Context.MobileInstallation.Count());
        Assert.Equal(3, _builder.Context.MobileLicences.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(7, _builder.Context.MobileModules.Count());
        Assert.Equal(5, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(2, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(_builder.Tenant2)));
        Assert.Equal(0, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(tenant3)));

        var response = new LicenceAdmin(_builder.Context, _authPlugin.AuthPlugin).SyncSystems(localSystems);
        Assert.Equal(2, response.Count);
        Assert.Equal(3, _builder.Context.Tenants.Count());
        Assert.Equal(3, _builder.Context.MobileInstallation.Count());
        Assert.Equal("#000000", _builder.Context.MobileInstallation.First(i => i.Url.Equals("Added3")).Color);
        Assert.Equal(10, _builder.Context.MobileModules.Count());
        Assert.Equal(5, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(2, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(_builder.Tenant2)));
        Assert.Equal(0, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(tenant3)));
    }

    [Fact]
    public void UpdateLicAndSystem()
    {
        var localSystems = BuildMobileLicenceSync();
        localSystems.Installations.Remove(localSystems.Installations.First(s => s.Id.Equals(_builder.Installation2.Id)));
        localSystems.Licences.Remove(localSystems.Licences.First(l => l.Licence.Equals("new")));
        localSystems.Modules.Remove(localSystems.Modules.First(l => l.Licence.Equals("new")));
        _builder.Context.SaveChanges();  // ensure no tracking
        Assert.Equal(2, _builder.Context.MobileInstallation.Count());
        Assert.Equal(3, _builder.Context.MobileLicences.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(5, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(_builder.Tenant1)));
        new LicenceAdmin(_builder.Context, _authPlugin.AuthPlugin).UpdateSystems(localSystems);
        Assert.Equal(2, _builder.Context.MobileInstallation.Count());
        Assert.Equal(3, _builder.Context.MobileLicences.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(5, _builder.Context.MobileModules.Count(m => m.Tenant.Equals(_builder.Tenant1)));
    }

    [Fact]
    public void CheckUpdateAddLic()
    {
        var localSystems = BuildMobileLicenceSync();
        localSystems.Tenants.First(t => t.Name.Equals("Tenant2")).Name = "Tenant2_Upd";
        var tenant3 = new Tenant
        {
            Name = "Tenant3"
        };
        localSystems.Tenants.Add(tenant3);
        var addedInstallation = new MobileInstallation
        {
            Url = "Added3",
            Tenant = tenant3
        };
        localSystems.Installations.Add(addedInstallation);
        localSystems.Licences.AddRange(new List<MobileLicence>
        {
            new()
            {
                Licence = "AddedLic1", LicenceStatus = LicenceStatusEnum.New, Tenant = tenant3
            },
            new()
            {
                Licence = "AddedLic2", LicenceStatus = LicenceStatusEnum.New, Tenant = tenant3
            },
            new()
            {
                Licence = "AddedTo1Lic", LicenceStatus = LicenceStatusEnum.New, Tenant = _builder.Tenant1
            }
        });
        foreach (var licence in localSystems.Licences)
        { 
            localSystems.LicenceUsers.Add(new User
            {
                Name = licence.Licence,
                Password = licence.Licence
            });
        }
        localSystems.Modules.AddRange(new List<MobileModule>
        {
            new()
            {
                Licence = "AddedLic1", ModuleName = "ArticleInformation", LicencedUntil = DateTime.Now.AddYears(1), Tenant = tenant3
            },
            new()
            {
                Licence = "AddedLic2",  ModuleName = "Inventory", LicencedUntil = DateTime.Now.AddYears(1), Tenant = tenant3
            },
            new()
            {
                Licence = "AddedTo1Lic",  ModuleName = "ArticleInformation", LicencedUntil = DateTime.Now.AddYears(1), Tenant = _builder.Tenant1
            }
        });

        localSystems.Licences.First(l => l.Licence.Equals("new")).DeviceInfo = "changed";
        localSystems.Licences.First(l => l.Licence.Equals("inUse1")).LicenceStatus = LicenceStatusEnum.Reset;
        _builder.Context.SaveChanges();  // ensure no tracking
        Assert.Equal(2, _builder.Context.Tenants.Count());
        Assert.Equal(2, _builder.Context.MobileInstallation.Count());
        Assert.Equal(3, _builder.Context.MobileLicences.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        Assert.Equal(5, _builder.Context.MobileModules.Count(l => l.Tenant.Equals(_builder.Tenant1)));
        new LicenceAdmin(_builder.Context, _authPlugin.AuthPlugin).UpdateSystems(localSystems);
        Assert.Equal(3, _builder.Context.Tenants.Count());
        Assert.Equal(3, _builder.Context.MobileInstallation.Count());
        Assert.Equal(7, _builder.Context.MobileLicences.Count());
        Assert.Equal(7, _authPlugin.Context.Users.Count());
        Assert.Equal(10, _builder.Context.MobileModules.Count());

    }

    private LicenceSync BuildMobileLicenceSync()
    {
        var mobSync = new LicenceSync
        {
            Tenants = _builder.Context.Tenants.AsNoTracking().ToList(),
            Installations = _builder.Context.MobileInstallation.AsNoTracking().ToList(),
            Licences = _builder.Context.MobileLicences.AsNoTracking().ToList(),
            LicenceUsers = new List<User>(),
            Modules = _builder.Context.MobileModules.AsNoTracking().ToList()
        };
        _builder.ResetContext();
        return mobSync;
    }
}

[UsedImplicitly]
public class AuthPluginFixture
{
    public AuthPluginFixture()
    {
        AuthPlugin = new UserDatabaseAuthenticationPlugin(new Mock<IHostEnvironment>().Object)
        {
            Config = {ConnectionString = $"Data Source={Path.GetTempFileName()}"}
        };

        Context = new UserDbContext(new DbContextOptionsBuilder<UserDbContext>().UseSqlite(
            AuthPlugin.Config.ConnectionString).Options, new ExplicitUserContext("Test", null));
        Context.Database.EnsureCreated();
        var scope1 = new Scope
        {
            Name = ScopeNamesInfrastructure.ReaderScope
        };
        var claimA = new Claim
        {
            Name = ClaimNamesInfrastructure.GeneralClaim,
            Scope = scope1,
            AvailablePermissions = PermissionKind.Read
        };

        Context.Scopes.Add(scope1);
        Context.Claims.Add(claimA);
        Context.SaveChanges();
    }

    public UserDatabaseAuthenticationPlugin AuthPlugin { get; }

    public UserDbContext Context { get; }
}