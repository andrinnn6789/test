using System;

using IAG.ControlCenter.Mobile.DataLayer.Context;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Test.Mobile.BusinessLayer;

public class TestContextBuilder: IDisposable
{
    private readonly DbContextOptions<MobileDbContext> _options;

    public Tenant Tenant1 { get; }
    public Tenant Tenant2 { get; }
    public MobileInstallation Installation1 { get; }
    public MobileInstallation Installation2 { get; }

    public MobileDbContext Context { get; private set; }

    public Guid InstallationGuid1 = Guid.NewGuid();

    public Guid InstallationGuid2 = Guid.NewGuid();

    public TestContextBuilder()
    {
        _options = new DbContextOptionsBuilder<MobileDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new MobileDbContext(_options, new ExplicitUserContext("test", null));
        Tenant1 = new Tenant { Id = Guid.NewGuid(), Name = "Tenant1" };
        Tenant2 = new Tenant { Id = Guid.NewGuid(), Name = "Tenant2" };
        Context.Tenants.Add(Tenant1);
        Installation1 = new MobileInstallation
        {
            Id = InstallationGuid1, 
            Url = "Tenant1-url1", 
            Tenant = Tenant1, 
            Name = "inst test", 
            SyncInterval = 10, 
            Color = "#0000FF"
        };
        Context.MobileInstallation.Add(Installation1);
        Installation2 = new MobileInstallation
        {
            Id = InstallationGuid2, 
            Url = "Tenant1-url2", 
            Tenant = Tenant1, 
            Name = "inst prod", 
            SyncInterval = 10,
            Color = "#0000FF"
        };
        Context.MobileInstallation.Add(Installation2);
        Context.MobileLicences.AddRange(
            new MobileLicence
            {
                Id = Guid.NewGuid(),
                Licence = "new",
                Tenant = Tenant1,
                LicenceStatus = LicenceStatusEnum.New
            },
            new MobileLicence
            {
                Id = Guid.NewGuid(),
                Licence = "inUse1",
                Tenant = Tenant1,
                LicenceStatus = LicenceStatusEnum.Inuse,
                DeviceId = "1",
                DeviceInfo = "info1"
            },
            new MobileLicence
            {
                Id = Guid.NewGuid(),
                Licence = "inUse2",
                Tenant = Tenant2,
                LicenceStatus = LicenceStatusEnum.Inuse,
                DeviceId = "2",
                DeviceInfo = "info2"
            },
            new MobileLicence
            {
                Id = Guid.NewGuid(),
                Licence = "revoked",
                Tenant = Tenant1,
                LicenceStatus = LicenceStatusEnum.Revoked
            }
        );

        Context.MobileModules.AddRange(
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "new",
                ModuleName = "ArticleInformation",
                LicencedUntil = DateTime.Today.AddYears(1),
                Tenant = Tenant1
            },
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "inUse1",
                ModuleName = "Inventory",
                LicencedUntil = DateTime.Today.AddYears(1),
                Tenant = Tenant1
            },
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "inUse1",
                ModuleName = "ArticleInformation",
                LicencedUntil = DateTime.Today.AddMonths(6),
                Tenant = Tenant1
            },
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "inUse1",
                ModuleName = "Inventory",
                LicencedUntil = DateTime.Today.AddDays(-1),
                Tenant = Tenant1
            },
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "inUse2",
                ModuleName = "ArticleInformation",
                Tenant = Tenant2
            },
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "inUse2",
                ModuleName = "ArticleInformation",
                LicencedUntil = DateTime.Today.AddMonths(6),
                Tenant = Tenant2
            },
            new MobileModule()
            {
                Id = Guid.NewGuid(),
                Licence = "revoked",
                ModuleName = "ArticleInformation",
                LicencedUntil = DateTime.Today.AddMonths(6),
                Tenant = Tenant1
            }
        );
            
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Dispose();
    }

    public void ResetContext()
    {
        Context?.Dispose();
        Context = new MobileDbContext(_options, new ExplicitUserContext("test", null));
    }
}