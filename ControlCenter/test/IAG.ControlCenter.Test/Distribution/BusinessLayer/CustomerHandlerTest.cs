using System;
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

public class CustomerHandlerTest : IDisposable
{
    private readonly DistributionDbContext _context;
    private readonly CustomerHandler _handler;
    private readonly Guid _testProductId = Guid.NewGuid();
    private readonly Guid _testReleaseId = Guid.NewGuid();
    private readonly Guid _testConfigurationId = Guid.NewGuid();
    private readonly Guid _testConfigurationReleaseId = Guid.NewGuid();
    private readonly Guid _testCustomerAlphaId = Guid.NewGuid();
    private readonly Guid _testCustomerBetaId = Guid.NewGuid();
    private readonly Guid _testFileId = Guid.NewGuid();

    public CustomerHandlerTest()
    {
        var options = new DbContextOptionsBuilder<DistributionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new DistributionDbContext(options, new ExplicitUserContext("test", null));
        _handler = new CustomerHandler(_context);

        _context.Products.Add(new Product { Id = _testProductId, Name = "TestProduct", ProductType = ProductType.IagService});
        _context.Products.Add(new Product { Id = _testConfigurationId, Name = "TestConfiguration", ProductType = ProductType.ConfigTemplate, DependsOnProductId = _testProductId});
        _context.Releases.Add(new Release { Id = _testReleaseId, ProductId = _testProductId, ReleaseVersion = "1.0", ReleaseDate = DateTime.Today.AddDays(-1)});
        _context.Releases.Add(new Release { Id = _testConfigurationReleaseId, ProductId = _testConfigurationId, ReleaseVersion = "1.0", ReleaseDate = DateTime.Today.AddDays(-1)});
        _context.Customers.Add(new Customer { Id = _testCustomerAlphaId, Name = "Alpha Company" });
        _context.Customers.Add(new Customer { Id = _testCustomerBetaId, Name = "Beta Company" });
        _context.ProductCustomers.Add(new ProductCustomer { ProductId = _testProductId, CustomerId = _testCustomerAlphaId });
        _context.FileStores.Add(new FileStore {Id = _testFileId, Name = "Base.dll", Data = Encoding.UTF8.GetBytes("TestContent")});
        _context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = _testReleaseId, FileStoreId = _testFileId });
        _context.Links.Add(new Link {Name = "TestLink", Url = "https://www.i-ag.ch"});

        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetCustomerTest()
    {
        var customerAlpha = await _handler.GetCustomerAsync(_testCustomerAlphaId);

        Assert.NotNull(customerAlpha);
        Assert.Equal(_testCustomerAlphaId, customerAlpha.Id);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetProductsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetProductsTest()
    {
        var productsAlpha = (await _handler.GetProductsAsync(_testCustomerAlphaId)).ToList();
        var productsBeta = (await _handler.GetProductsAsync(_testCustomerBetaId)).ToList();

        Assert.Equal(2, productsAlpha.Count);
        Assert.Contains(productsAlpha, p => p.Id == _testProductId);
        Assert.Contains(productsAlpha, p => p.Id == _testConfigurationId);
        Assert.Empty(productsBeta);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetProductsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetReleasesTest()
    {
        var releases = (await _handler.GetReleasesAsync(_testCustomerAlphaId, _testProductId)).ToList();
        var configReleases = (await _handler.GetReleasesAsync(_testCustomerAlphaId, _testConfigurationId)).ToList();

        Assert.Equal(_testReleaseId, Assert.Single(releases)?.Id);
        Assert.Equal(_testConfigurationReleaseId, Assert.Single(configReleases)?.Id);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleasesAsync(Guid.NewGuid(), Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleasesAsync(_testCustomerAlphaId, Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleasesAsync(Guid.NewGuid(), _testProductId));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleasesAsync(_testCustomerBetaId, _testProductId));
    }

    [Fact]
    public async Task GetReleaseFilesTest()
    {
        var files = (await _handler.GetReleaseFilesAsync(_testCustomerAlphaId, _testReleaseId)).ToList();
        var configFiles = (await _handler.GetReleaseFilesAsync(_testCustomerAlphaId, _testConfigurationReleaseId)).ToList();

        Assert.Equal(_testFileId, Assert.Single(files)?.Id);
        Assert.Empty(configFiles);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleaseFilesAsync(Guid.NewGuid(), Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleaseFilesAsync(_testCustomerAlphaId, Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleaseFilesAsync(_testCustomerAlphaId, Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetReleaseFilesAsync(_testCustomerBetaId, _testReleaseId));
    }

    [Fact]
    public async Task GetFileContentTest()
    {
        var file = await _handler.GetFileAsync(_testCustomerAlphaId, _testFileId);

        Assert.Equal(_testFileId, file.Id);
        Assert.Equal(Encoding.UTF8.GetBytes("TestContent"), file.Content);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetFileAsync(Guid.NewGuid(), Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetFileAsync(_testCustomerAlphaId, Guid.NewGuid()));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetFileAsync(Guid.NewGuid(), _testFileId));
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetFileAsync(_testCustomerBetaId, _testFileId));
    }

    [Fact]
    public async Task RegisterInstallationTest()
    {
        var testInstallation = new InstallationRegistration
        {
            InstanceName = "TestInstallation",
            Description = "TestComment",
            Platform = "win-x64",
            ProductId = _testProductId
        };
        var transferInstallation = new InstallationRegistration
        {
            InstanceName = "TransferInstallation",
            Platform = "win-x64",
            Description = "Transferred"
        };
        var testUnInstallation = new InstallationRegistration
        {
            InstanceName = "TestInstallation",
            Platform = "win-x64",
            Description = "Uninstalled"
        };
        var noNameInstallation = new InstallationRegistration
        {
            ProductId = _testProductId
        };
        var noPathInstallation = new InstallationRegistration
        {
            ProductId = _testProductId
        };
 
        var installation = await _handler.RegisterInstallationAsync(_testCustomerAlphaId, testInstallation);
        var transfer = await _handler.RegisterInstallationAsync(_testCustomerAlphaId, transferInstallation);
        var unInstallation = await _handler.RegisterInstallationAsync(_testCustomerAlphaId, testUnInstallation);

        Assert.NotNull(installation);
        Assert.NotNull(transfer);
        Assert.NotNull(unInstallation);
        Assert.NotEqual(installation.Id, transfer.Id);
        Assert.NotEqual(installation.Id, unInstallation.Id);
        Assert.NotEqual(transfer.Id, unInstallation.Id);
        Assert.Equal(_testCustomerAlphaId, installation.CustomerId);
        Assert.Equal(_testCustomerAlphaId, transfer.CustomerId);
        Assert.Equal(_testCustomerAlphaId, unInstallation.CustomerId);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.RegisterInstallationAsync(Guid.NewGuid(), testInstallation));
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.RegisterInstallationAsync(_testCustomerAlphaId, noNameInstallation));
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.RegisterInstallationAsync(_testCustomerAlphaId, noPathInstallation));
    }

    [Fact]
    public async Task GetLinksTest()
    {
        var linksAlpha = (await _handler.GetLinksAsync(_testCustomerAlphaId)).ToList();
        var linksBeta = (await _handler.GetLinksAsync(_testCustomerBetaId)).ToList();

        var linkAlpha = Assert.Single(linksAlpha);
        var linkBeta = Assert.Single(linksBeta);
        Assert.NotNull(linkAlpha);
        Assert.NotNull(linkBeta);
        Assert.Equal(linkAlpha.Id, linkBeta.Id);
        Assert.Equal("TestLink", linkAlpha.Name);
        Assert.Equal("TestLink", linkBeta.Name);
        Assert.Equal("https://www.i-ag.ch", linkAlpha.Url);
        Assert.Equal("https://www.i-ag.ch", linkBeta.Url);
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _handler.GetLinksAsync(Guid.NewGuid()));
    }
}