using System;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.MockHost;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.IntegrationTest.BusinessLogic;

public class ReleaseManagerTest
{
    private static readonly Guid TestCustomerId = Guid.Parse("68b7efca-ca3f-454e-977d-0ff767fad44b");
    private static readonly Guid TestProductId = Guid.Parse("38f7f47f-998d-44d6-984e-db97bca8cae9");
    private static readonly Guid TestReleaseId = Guid.Parse("6f653d78-cf6b-422b-bcdc-6026b17d6ee3");
    private static readonly Guid TestFileId = Guid.Parse("e6963108-5dd6-454b-a118-87958ad4625a");

    private readonly IReleaseManager _releaseManager;

    public ReleaseManagerTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".ReleaseManagerRequestMock.json", testPort);

        var config = new Mock<IConfiguration>();
        config.SetupGet(m => m["ControlCenter:BaseUrl"]).Returns($"http://localhost:{testPort}");
            
        _releaseManager = new ReleaseManager(config.Object);
    }

    [Fact]
    public async Task GetProductsTest()
    {
        var products = await _releaseManager.GetProductsAsync(TestCustomerId);

        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task GetReleasesTest()
    {
        var releases = await _releaseManager.GetReleasesAsync(TestCustomerId, TestProductId);

        Assert.NotEmpty(releases);
    }

    [Fact]
    public async Task GetReleaseFilesTest()
    {
        var files = await _releaseManager.GetReleaseFilesAsync(TestCustomerId, TestProductId, TestReleaseId);

        Assert.NotEmpty(files);
    }

    [Fact]
    public async Task GetFileContentTest()
    {
        var contentStream = await _releaseManager.GetFileContentStreamAsync(TestCustomerId, TestFileId);

        Assert.NotNull(contentStream);
    }
}