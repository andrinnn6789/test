using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

[CustomerPluginInfo(CustomerId)]
public static class CustomerPluginInfo
{
    public const string CustomerId = "D527054E-F344-46FE-B615-DD5ADD4D2DA3";
}

public class ReleaseManagerTest
{
    private readonly ReleaseManager _releaseManager;
    private readonly Mock<IProductAdminClient> _productAdminClientMock;
    private readonly Mock<ICustomerAdminClient> _customerAdminClientMock;

    public ReleaseManagerTest()
    {
        _productAdminClientMock = new Mock<IProductAdminClient>();
        _customerAdminClientMock = new Mock<ICustomerAdminClient>();

        _releaseManager = new ReleaseManager(_productAdminClientMock.Object, _customerAdminClientMock.Object, new Mock<IMessageLogger>().Object);
    }

    [Fact]
    public async Task GetProductsTest()
    {
        var productId = Guid.NewGuid();
            
        _productAdminClientMock.Setup(m => m.GetProductsAsync())
            .ReturnsAsync(() => new List<ProductInfo>
            {
                new()
                {
                    Id = productId,
                }
            });


        var products = await _releaseManager.GetProductsAsync();
            
        var singleProduct = Assert.Single(products);
        Assert.NotNull(singleProduct);
        Assert.Equal(productId, singleProduct.Id);
    }

    [Fact]
    public async Task GetReleasesTest()
    {
        var releaseId = Guid.NewGuid();

        _productAdminClientMock.Setup(m => m.GetReleasesAsync())
            .ReturnsAsync(() => new List<ReleaseInfo>
            {
                new()
                {
                    Id = releaseId,
                }
            });


        var releases = await _releaseManager.GetReleasesAsync();

        var singleRelease = Assert.Single(releases);
        Assert.NotNull(singleRelease);
        Assert.Equal(releaseId, singleRelease.Id);
    }

    [Fact]
    public async Task RegisterProductTest()
    {
        var productId = Guid.NewGuid();
        var dependingProductId = Guid.NewGuid();

        _productAdminClientMock.Setup(m => m.RegisterProductAsync(It.IsAny<string>(), It.IsAny<ProductType>(), It.IsAny<Guid?>()))
            .ReturnsAsync((string name, ProductType type, Guid? dependsOn) => new ProductInfo
            {
                Id = productId,
                ProductName = name,
                ProductType = type,
                DependsOnProductId = dependsOn
            });


        var product = await _releaseManager.CreateProductAsync("TestProduct", ProductType.IagService, dependingProductId);

        Assert.NotNull(product);
        Assert.Equal("TestProduct", product.ProductName);
        Assert.Equal(ProductType.IagService, product.ProductType);
        Assert.Equal(dependingProductId, product.DependsOnProductId);
    }

    [Fact]
    public async Task RegisterReleaseTest()
    {
        ReleaseInfo registeredRelease = null;
        byte[] registeredFileContent = null;

        var productId = Guid.NewGuid();
        var product = new ProductInfo
        {
            Id = productId,
            ProductName = "TestProduct",
            ProductType = ProductType.CustomerExtension
        };

        List<Guid> customerAssignedProducts = null;
        var addedDepsJson = true;
        var approved = false;
        var releaseId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _productAdminClientMock.Setup(m => m.RegisterReleaseAsync(productId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Guid _, string release, string platform, string description, string releasePath, string artifactPath) =>
            {
                registeredRelease = new ReleaseInfo
                {
                    Id = releaseId,
                    ReleaseVersion = release,
                    Platform = platform,
                    ReleasePath = releasePath,
                    ArtifactPath = artifactPath,
                    Description = description
                };
                return registeredRelease;
            });
        _productAdminClientMock.Setup(m => m.AddFilesToReleaseAsync(productId, releaseId, It.IsAny<List<FileRegistration>>()))
            .ReturnsAsync((Guid _, Guid _, List<FileRegistration> files) =>
            {
                addedDepsJson = files.Any(f => f.Name.EndsWith(".deps.json"));

                return files.Where(f => f.Name == "IAG.VinX.IAG.Test.dll")
                    .Select(x => new FileMetaInfo {Id = fileId, Name = x.Name}).ToList();
            });
        _productAdminClientMock.Setup(m => m.SetFileContentAsync(fileId, It.IsAny<byte[]>()))
            .Callback((Guid _, byte[] content) =>
            {
                registeredFileContent = content;
            });
        _productAdminClientMock.Setup(m => m.ApproveReleaseAsync(productId, releaseId))
            .Callback(() => { approved = true; });

        _customerAdminClientMock.Setup(m => m.AddProductsAsync(new Guid(CustomerPluginInfo.CustomerId), It.IsAny<IEnumerable<Guid>>()))
            .Callback<Guid, IEnumerable<Guid>>((_, products) => customerAssignedProducts = products.ToList());

        var artifactsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            PrepareTestDirectory(artifactsPath);
            await _releaseManager.CreateReleaseAsync(product, artifactsPath, "ReleasePath", "1.0",
                new Mock<IJobHeartbeatObserver>().Object);

            Assert.NotNull(registeredRelease);
            Assert.NotEmpty(registeredRelease.Platform);
            Assert.NotEmpty(registeredRelease.ReleaseVersion);
            Assert.NotEmpty(registeredRelease.Description);
            Assert.Equal("ReleasePath", registeredRelease.ReleasePath);
            Assert.Equal(artifactsPath, registeredRelease.ArtifactPath);
            Assert.NotNull(registeredFileContent);
            Assert.NotNull(customerAssignedProducts);
            Assert.Equal(productId, Assert.Single(customerAssignedProducts));
            Assert.False(addedDepsJson);
            Assert.True(approved);
        }
        finally
        {
            Directory.Delete(artifactsPath, true);
        }
    }

    [Fact]
    public async Task ReleaseAlreadyApprovedErrorTest()
    {
        _productAdminClientMock.Setup(m => m.RegisterReleaseAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new ReleaseInfo { ReleaseDate = DateTime.Today });

        var artifactsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        await Assert.ThrowsAsync<LocalizableException>(() => _releaseManager.CreateReleaseAsync(new ProductInfo(), artifactsPath, "ReleasePath", null, new Mock<IJobHeartbeatObserver>().Object));
    }

    [Fact]
    public async Task DisableReleasesTest()
    {
        await _releaseManager.RemoveReleaseAsync(new ReleaseInfo());
    }


    private void PrepareTestDirectory(string testPath)
    {
        var assemblyDirectory = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
        Directory.CreateDirectory(testPath);

        foreach (var file in Directory.GetFiles(assemblyDirectory))
        {
            File.Copy(file, Path.Combine(testPath, Path.GetFileName(file)));
        }
    }
}