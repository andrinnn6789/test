using System;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.IntegrationTest.BusinessLogic;

public class CustomerManagerTest
{
    [Fact]
    public async Task GetCustomerInformationTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".CustomerManagerRequestMock.json", testPort);

        var config = new Mock<IConfiguration>();
        config.SetupGet(m => m["ControlCenter:BaseUrl"]).Returns($"http://localhost:{testPort}");
        var customerManager = new CustomerManager(config.Object);

        var customerInfo = await customerManager.GetCustomerInformationAsync(Guid.Parse("68b7efca-ca3f-454e-977d-0ff767fad44b"));

        Assert.NotNull(customerInfo);
        await Assert.ThrowsAsync<LocalizableException>(() => customerManager.GetCustomerInformationAsync(Guid.Empty));
        await Assert.ThrowsAsync<LocalizableException>(() => customerManager.GetCustomerInformationAsync(Guid.NewGuid()));
    }
}