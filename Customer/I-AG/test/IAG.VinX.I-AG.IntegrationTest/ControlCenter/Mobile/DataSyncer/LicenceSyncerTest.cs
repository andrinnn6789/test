using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Mobile.BusinessLayer;
using IAG.VinX.IAG.ControlCenter.Mobile.DataSyncer;
using IAG.VinX.IAG.IntegrationTest.ControlCenter.Mobile.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Mobile.DataSyncer;

public class LicenceSyncerTest
{
    private readonly IHttpConfig _httpConfig;

    public LicenceSyncerTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(typeof(LicenceSyncerJobTest).Namespace + ".RequestMock.json", testPort);
        _httpConfig = new HttpConfig
        {
            BaseUrl = $"http://localhost:{testPort}/" + Endpoints.Control
        };
    }

    [Fact]
    public async Task SyncOkDiffTest()
    {
        var vinx = new Mock<IVinXConnectorIag>();
        vinx.Setup(v => v.GetLicenceConfig(It.IsAny<DateTime>())).Returns(new LicenceSync
        {
            Tenants = new List<Tenant>(),
            Licences = new List<MobileLicence>
            {
                new()
            },
            Installations = new List<MobileInstallation>(),
            Modules = new List<MobileModule>()
            {
                new()
            }
        });
        var syncer = new LicenceSyncer(vinx.Object, _httpConfig, new MockILogger<LicenceSyncer>(), DateTime.MinValue, false);
        Assert.Equal(1, await syncer.Sync());
    }

    [Fact]
    public async Task SyncOkDiffNopTest()
    {
        var vinx = new Mock<IVinXConnectorIag>();
        vinx.Setup(v => v.GetLicenceConfig(It.IsAny<DateTime>())).Returns(new LicenceSync
        {
            Tenants = new List<Tenant>(),
            Licences = new List<MobileLicence>(),
            Installations = new List<MobileInstallation>(),
            Modules = new List<MobileModule>()
        });
        var syncer = new LicenceSyncer(vinx.Object, _httpConfig, new MockILogger<LicenceSyncer>(), DateTime.MinValue, false);
        Assert.Equal(0, await syncer.Sync());
    }

    [Fact]
    public async Task SyncOkFullTest()
    {
        var vinx = new Mock<IVinXConnectorIag>();
        vinx.Setup(v => v.GetLicenceConfig(It.IsAny<DateTime>())).Returns(new LicenceSync
        {
            Tenants = new List<Tenant>(),
            Licences = new List<MobileLicence>(),
            Installations = new List<MobileInstallation>(),
            Modules = new List<MobileModule>()
        });
        var syncer = new LicenceSyncer(vinx.Object, _httpConfig, new MockILogger<LicenceSyncer>(), DateTime.MinValue, true);
        Assert.Equal(1, await syncer.Sync());
    }

    [Fact]
    public async Task SyncNokTest()
    {
        var vinx = new Mock<IVinXConnectorIag>();
        vinx.Setup(v => v.GetLicenceConfig(It.IsAny<DateTime>())).Returns(new LicenceSync
        {
            Tenants = new List<Tenant>(),
            Licences = new List<MobileLicence>(),
            Installations = new List<MobileInstallation>(),
            Modules = new List<MobileModule>()
        });
        vinx.Setup(v => v.UpdateLicence(It.IsAny<MobileLicence>())).Throws<KeyNotFoundException>();
        var syncer = new LicenceSyncer(vinx.Object, _httpConfig, new MockILogger<LicenceSyncer>(), DateTime.MinValue, true);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => syncer.Sync());
    }
}