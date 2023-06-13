using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.ControlCenter.Test.Distribution.BusinessLayer;

public class ProductAdminHandlerTest : IDisposable
{
    private readonly DistributionDbContext _context;
    private readonly ProductAdminHandler _handler;

    public ProductAdminHandlerTest()
    {
        var options = new DbContextOptionsBuilder<DistributionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new DistributionDbContext(options, new ExplicitUserContext("test", null));
        _handler = new ProductAdminHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetProductsTest()
    {
        var testProduct = new Product
        {
            Name = "InitialTest",
            ProductType = ProductType.ThirdParty,
        };
        await _context.Products.AddAsync(testProduct);
        await _context.SaveChangesAsync();

        var products = await _handler.GetProductsAsync();

        _context.Products.Remove(testProduct);
        await _context.SaveChangesAsync();

        var singleProduct = Assert.Single(products);
        Assert.NotNull(singleProduct);
        Assert.Equal(testProduct.Id, singleProduct.Id);
        Assert.Equal(testProduct.Name, singleProduct.ProductName);
        Assert.Equal(testProduct.ProductType, singleProduct.ProductType);
    }

    [Fact]
    public async Task GetReleasesTest()
    {
        var testRelease = new Release
        {
            ReleaseVersion = "0.9"
        };
        await _context.Releases.AddAsync(testRelease);
        await _context.SaveChangesAsync();

        var releases = await _handler.GetReleasesAsync();

        _context.Releases.Remove(testRelease);
        await _context.SaveChangesAsync();

        var singleRelease = Assert.Single(releases);
        Assert.NotNull(singleRelease);
        Assert.Equal(testRelease.Id, singleRelease.Id);
        Assert.Equal(testRelease.ReleaseVersion, singleRelease.ReleaseVersion);
    }

    [Fact]
    public async Task RegisterProductTest()
    {
        var productRequest = new ProductRegistration
        {
            ProductName = "TestProduct",
            Description = "TestProductDescription",
            Type = ProductType.IagService
        };
        var product = await _handler.RegisterProductAsync(productRequest);

        var pluginRequest = new ProductRegistration
        {
            ProductName = "TestPlugin",
            Description = "TestPluginDescription",
            Type = ProductType.CustomerExtension,
            DependsOnProductId = product?.Id
        };
        var plugin = await _handler.RegisterProductAsync(pluginRequest);

        var productUpdater = new ProductRegistration
        {
            ProductName = "Updater",
            Description = "UpdaterDescription",
            Type = ProductType.Updater
        };
        await _context.Customers.AddAsync(new Customer());
        await _context.SaveChangesAsync();
        var updater = await _handler.RegisterProductAsync(productUpdater);
        _ = await _handler.RegisterProductAsync(productUpdater);  // for coverage

        Assert.NotNull(product);
        Assert.NotNull(plugin);
        Assert.NotNull(updater);
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.NotEqual(Guid.Empty, plugin.Id);
        Assert.Equal("TestProduct", product.ProductName);
        Assert.Equal("TestPlugin", plugin.ProductName);
        Assert.Equal("TestProductDescription", product.Description);
        Assert.Equal("TestPluginDescription", plugin.Description);
        Assert.Equal(ProductType.IagService, product.ProductType);
        Assert.Equal(ProductType.CustomerExtension, plugin.ProductType);
        Assert.Null(product.DependsOnProductId);
        Assert.Equal(product.Id, plugin.DependsOnProductId);
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterProductAsync(null));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterProductAsync(new ProductRegistration()));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterProductAsync(new ProductRegistration { ProductName = string.Empty}));
    }

    [Fact]
    public async Task RegisterReleaseTest()
    {
        var productRequest = new ProductRegistration { ProductName = "TestProduct" };
        var product = await _handler.RegisterProductAsync(productRequest);

        var releaseRequest = new ReleaseRegistration
        {
            Description = "TestDescription",
            ReleaseVersion = "Test 1.0",
            Platform = "win-x64",
            ReleasePath = "./TestPath/",
            ArtifactPath = "TestArtifactPath"
        };
        var release = await _handler.RegisterReleaseAsync(product.Id, releaseRequest);
            
        Assert.NotNull(release);
        Assert.NotEqual(Guid.Empty, release.Id);
        Assert.Equal(product.Id, release.ProductId);
        Assert.Null(release.ReleaseDate);
        Assert.Equal("TestDescription", release.Description);
        Assert.Equal("Test 1.0", release.ReleaseVersion);
        Assert.Equal("./TestPath/", release.ReleasePath);
        Assert.Equal("TestArtifactPath", release.ArtifactPath);
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.RegisterReleaseAsync(Guid.NewGuid(), releaseRequest));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterReleaseAsync(product.Id, new ReleaseRegistration()));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterReleaseAsync(product.Id, new ReleaseRegistration { ReleaseVersion = string.Empty }));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterReleaseAsync(product.Id, new ReleaseRegistration { ReleaseVersion = "2.3" }));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterReleaseAsync(product.Id, new ReleaseRegistration { ReleaseVersion = "2.3", Platform = string.Empty}));
    }

    [Fact]
    public async Task AddFilesToReleaseTest()
    {
        var files = new List<FileRegistration>
        {
            new()
            {
                Name = "Test.dll",
                ProductVersion = "1.1.0",
                FileVersion = "1.1.0.1",
                Checksum = new byte[] {1, 2, 3},
            },
            new()
            {
                Name = "Image.jpg",
                Checksum = new byte[] {4, 5, 6}
            }
        };

        var productRequest = new ProductRegistration { ProductName = "TestProduct" };
        var product = await _handler.RegisterProductAsync(productRequest);
        var releaseRequest = new ReleaseRegistration { ReleaseVersion = "Test 1.1", Platform = "win-x64" };
        var release = await _handler.RegisterReleaseAsync(product.Id, releaseRequest);

        var fileInfosFirstAdd = (await _handler.AddFilesToReleaseAsync(release.Id, files))?.ToList();
        var fileTestDll = fileInfosFirstAdd?.FirstOrDefault(f => f.Name == "Test.dll");
        var fileImageDll = fileInfosFirstAdd?.FirstOrDefault(f => f.Name == "Image.jpg");

        var fileInfosSecondAdd = (await _handler.AddFilesToReleaseAsync(release.Id, files))?.ToList();

        Assert.NotNull(fileInfosFirstAdd);
        Assert.NotNull(fileTestDll);
        Assert.NotNull(fileImageDll);
        Assert.Equal(2, fileInfosFirstAdd.Count);
        Assert.NotNull(fileInfosSecondAdd);
        Assert.Equal(2, fileInfosSecondAdd.Count);
        Assert.Equal("1.1.0", fileTestDll.ProductVersion);
        Assert.Equal("1.1.0.1", fileTestDll.FileVersion);
        Assert.Equal(new byte[] { 1, 2, 3 }, fileTestDll.Checksum);
        Assert.Equal(new byte[] { 4, 5, 6 }, fileImageDll.Checksum);
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.AddFilesToReleaseAsync(Guid.NewGuid(), null));
    }

    [Fact]
    public async Task SetFileContentTest()
    {
        var productRequest = new ProductRegistration { ProductName = "TestProduct" };
        var product = await _handler.RegisterProductAsync(productRequest);
        var releaseRequest = new ReleaseRegistration { ReleaseVersion = "Test 1.2", Platform = "win-x64" };
        var release = await _handler.RegisterReleaseAsync(product.Id, releaseRequest);
        var fileRequests = new List<FileRegistration> {new() {Name = "Test.dll"}};
        var file = (await _handler.AddFilesToReleaseAsync(release.Id, fileRequests)).First();

        var content = Encoding.UTF8.GetBytes("TestContent");
        var result = await _handler.SetFileContentAsync(file.Id, content);

        Assert.NotNull(result);
        Assert.Equal(file.Id, result.Id);
        Assert.Equal("Test.dll", result.Name);
        Assert.NotNull(result.Checksum);
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.SetFileContentAsync(Guid.NewGuid(), null));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.SetFileContentAsync(result.Id, null));
    }

    [Fact]
    public async Task ApproveReleaseTest()
    {
        var productRequest = new ProductRegistration { ProductName = "TestProduct" };
        var product = await _handler.RegisterProductAsync(productRequest);
        var releaseRequest = new ReleaseRegistration { ReleaseVersion = "Test 1.0", Platform = "win-x64" };
        var release = await _handler.RegisterReleaseAsync(product.Id, releaseRequest);

        var files = new List<FileRegistration>
        {
            new() { Name = "TestData.json", Checksum = new byte[] {7, 8, 9} }
        };
        var file = (await _handler.AddFilesToReleaseAsync(release.Id, files)).First();

        var content = Encoding.UTF8.GetBytes("TestContent");
        await _handler.SetFileContentAsync(file.Id, content);

        var approvedRelease = await _handler.ApproveReleaseAsync(release.Id);

        Assert.Null(release.ReleaseDate);
        Assert.NotNull(approvedRelease);
        Assert.NotEqual(Guid.Empty, approvedRelease.Id);
        Assert.Equal("Test 1.0", approvedRelease.ReleaseVersion);
        Assert.NotNull(approvedRelease.ReleaseDate);
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.ApproveReleaseAsync(Guid.NewGuid()));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.ApproveReleaseAsync(release.Id));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.AddFilesToReleaseAsync(release.Id, new FileRegistration[0]));
    }

    [Fact]
    public async Task ApproveUnfinishedReleaseFailureTest()
    {
        var productRequest = new ProductRegistration { ProductName = "TestProduct" };
        var product = await _handler.RegisterProductAsync(productRequest);
        var releaseRequest = new ReleaseRegistration { ReleaseVersion = "Test 1.0.1", Platform = "win-x64" };
        var release = await _handler.RegisterReleaseAsync(product.Id, releaseRequest);

        var files = new List<FileRegistration>
        {
            new() { Name = "TestData2.json", Checksum = new byte[] {7, 8, 9} }
        };
        await _handler.AddFilesToReleaseAsync(release.Id, files);

        await Assert.ThrowsAsync<BadRequestException>(() => _handler.ApproveReleaseAsync(release.Id));
    }

    [Fact]
    public async Task DisableReleaseTest()
    {
        var productRequest = new ProductRegistration { ProductName = "TestProduct" };
        var product = await _handler.RegisterProductAsync(productRequest);
        var releaseRequest = new ReleaseRegistration { ReleaseVersion = "Test 1.0", Platform = "win-x64" };
        var release = await _handler.RegisterReleaseAsync(product.Id, releaseRequest);

        await _handler.RemoveReleaseAsync(release.Id);

        Assert.Null(release.ReleaseDate);
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.RemoveReleaseAsync(Guid.NewGuid()));
    }
}