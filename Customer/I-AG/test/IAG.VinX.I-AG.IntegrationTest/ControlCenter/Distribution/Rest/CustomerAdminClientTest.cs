using System;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.Rest;

public class CustomerAdminClientTest
{
    [Fact]
    public async Task HappyPathTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".CustomerAdminRequestMock.json", testPort);

        var httpConfig = new HttpConfig() { BaseUrl = $"http://localhost:{testPort}/{Endpoints.Distribution}" };
        var client = new CustomerAdminClient(httpConfig);

        var customer = await client.RegisterCustomerAsync(Guid.NewGuid(), "TestCustomer", 42, string.Empty);
        await client.GetCustomersAsync();
        await client.AddProductsAsync(customer.Id, new[] { Guid.NewGuid() });
    }

    [Fact]
    public async Task ErrorsTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".CustomerAdminRequestMock.json", testPort);

        var httpConfig = new HttpConfig() { BaseUrl = $"http://localhost:{testPort}/WrongEndpoint" };
        var client = new CustomerAdminClient(httpConfig);

        await Assert.ThrowsAsync<LocalizableException>(() => client.RegisterCustomerAsync(Guid.Empty, string.Empty, 0, string.Empty));
        await Assert.ThrowsAsync<LocalizableException>(() => client.GetCustomersAsync());
        await Assert.ThrowsAsync<LocalizableException>(() => client.AddProductsAsync(Guid.Empty, new[] { Guid.Empty }));
    }
}