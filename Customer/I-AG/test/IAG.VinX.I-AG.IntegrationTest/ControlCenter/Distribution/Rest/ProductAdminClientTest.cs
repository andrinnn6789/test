using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.Rest;

public class ProductAdminClientTest
{
    private readonly ProductAdminClient _client;

    public ProductAdminClientTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".ProductAdminRequestMock.json", testPort);

        var httpConfig = new HttpConfig { BaseUrl = $"http://localhost:{testPort}/{Endpoints.Distribution}" };

        _client = new ProductAdminClient(httpConfig);
    }

    [Fact]
    public async Task HappyPathTest()
    {
        var products = await _client.GetProductsAsync();
        var releases = await _client.GetReleasesAsync();
        var productInfo = await _client.RegisterProductAsync("TestProduct", ProductType.IagService);
        var releaseInfo = await _client.RegisterReleaseAsync(productInfo.Id, "1.0.0", "Windows", "TestDescription", "TestReleasePath", "TestArtifactsPath");
        var filesToAdd = await _client.AddFilesToReleaseAsync(productInfo.Id, releaseInfo.Id, new List<FileRegistration> {new() {Name = "Test.txt"}});
        await _client.SetFileContentAsync(filesToAdd.Single().Id, new byte[] { });
        await _client.ApproveReleaseAsync(productInfo.Id, releaseInfo.Id);
        await _client.RemoveReleaseAsync(productInfo.Id, releaseInfo.Id);

        Assert.NotNull(products);
        Assert.Empty(products);
        Assert.NotNull(releases);
        Assert.Empty(releases);
        Assert.NotNull(productInfo);
        Assert.NotNull(releaseInfo);
        Assert.NotNull(filesToAdd);
        Assert.NotEmpty(filesToAdd);
    }

    [Fact]
    public async Task ErrorsTest()
    {
        await Assert.ThrowsAsync<LocalizableException>(() => _client.RegisterProductAsync("InvalidProduct", ProductType.IagService));
        await Assert.ThrowsAsync<LocalizableException>(() => _client.RegisterReleaseAsync(Guid.NewGuid(), "1.0.0", "Windows", "TestDescription", "TestReleasePath", "TestArtifactsPath"));
        await Assert.ThrowsAsync<LocalizableException>(() => _client.AddFilesToReleaseAsync(Guid.NewGuid(), Guid.Empty, new List<FileRegistration>()));
        await Assert.ThrowsAsync<LocalizableException>(() => _client.SetFileContentAsync(Guid.NewGuid(), new byte[] { }));
        await Assert.ThrowsAsync<LocalizableException>(() => _client.ApproveReleaseAsync(Guid.NewGuid(), Guid.Empty));
        await Assert.ThrowsAsync<LocalizableException>(() => _client.RemoveReleaseAsync(Guid.NewGuid(), Guid.Empty));
    }

    [Fact]
    public async Task GetProductsAndReleasesErrorTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".ProductAdminRequestMock2.json", testPort);

        var httpConfig = new HttpConfig() { BaseUrl = $"http://localhost:{testPort}/{Endpoints.Distribution}" };
        var client = new ProductAdminClient(httpConfig);

        await Assert.ThrowsAsync<LocalizableException>(() => client.GetProductsAsync());
        await Assert.ThrowsAsync<LocalizableException>(() => client.GetReleasesAsync());
    }

}