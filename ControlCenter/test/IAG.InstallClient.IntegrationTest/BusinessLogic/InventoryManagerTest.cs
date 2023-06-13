using System;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.IntegrationTest.BusinessLogic;

public class InventoryManagerTest
{
    private static readonly Guid TestCustomerId = Guid.Parse("68b7efca-ca3f-454e-977d-0ff767fad44b");

    private readonly IInventoryManager _inventoryManager;

    public InventoryManagerTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".InventoryManagerRequestMock.json", testPort);

        var config = new Mock<IConfiguration>();
        config.SetupGet(m => m["ControlCenter:BaseUrl"]).Returns($"http://localhost:{testPort}");
            
        _inventoryManager = new InventoryManager(config.Object);
    }

    [Fact]
    public async Task RegisterInstallationTest()
    {
        var installation = await _inventoryManager.RegisterInstallationAsync(
            TestCustomerId, 
            new InstallationRegistration
            {
                ProductId = Guid.NewGuid(),
                InstanceName = "instance"
            }
        );

        Assert.NotNull(installation);
    }

    [Fact]
    public async Task DeRegisterInstallationTest()
    {
        var installation = await _inventoryManager.DeRegisterInstallationAsync(TestCustomerId, "TestInstanceName");

        Assert.NotNull(installation);
    }
}