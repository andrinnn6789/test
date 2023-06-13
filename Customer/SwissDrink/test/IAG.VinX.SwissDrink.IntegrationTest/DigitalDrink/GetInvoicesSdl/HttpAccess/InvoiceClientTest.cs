using System;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;

using Moq;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.DigitalDrink.GetInvoicesSdl.HttpAccess;

public class InvoiceClientTest
{
    [Fact]
    public async Task GetInvoicesSdlSuccessTest()
    {
        var config = DdMiddlewareConfigHelper.Config;
        config.KestrelMockRequestJson = "GetInvoicesSdlRequestMock.json";
        var invoiceClient = CreateInvoiceClient(config);

        var results = (await invoiceClient.GetInvoicesSdl(DateTime.Today.AddDays(-10)))?.ToList();

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.NotEmpty(results.First().ArticlePositions);
        Assert.NotNull(results.First().PackagePositions);
    }

    [Fact]
    public async Task GetInvoicesSdlFailureTest()
    {
        var config = DdMiddlewareConfigHelper.Config;
        config.KestrelMockRequestJson = "GetInvoicesSdlRequestFailureMock.json";
        var invoiceClient = CreateInvoiceClient(config);

        await Assert.ThrowsAsync<RestException>(() => invoiceClient.GetInvoicesSdl(DateTime.Today.AddYears(100)));
    }

    private IInvoiceClient CreateInvoiceClient(DdMiddlewareConfig config)
    {
        if (!config.UseRealSystem)
        {
            var testPort = KestrelMock.NextFreePort;
            KestrelMock.Run($"{GetType().Namespace}.{config.KestrelMockRequestJson}", testPort);
            config.BaseUrl = $"http://localhost:{testPort}";
        }

        var httpConfigMock = new Mock<IHttpConfig>();
        httpConfigMock.Setup(m => m.BaseUrl).Returns(config.BaseUrl);
        if (!string.IsNullOrEmpty(config.UserName))
        {
            httpConfigMock.Setup(m => m.Authentication).Returns(new BasicAuthentication()
            {
                User = config.UserName,
                Password = config.Password
            });
        }

        var client = new InvoiceClient();
        client.InitClient(null, httpConfigMock.Object);

        return client;
    }
}