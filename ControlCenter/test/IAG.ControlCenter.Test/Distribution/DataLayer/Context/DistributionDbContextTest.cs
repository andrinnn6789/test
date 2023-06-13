using System;
using System.Text;

using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.ControlCenter.Test.Distribution.DataLayer.Context;

public class DistributionDbContextTest
{
    [Fact]
    public void SimpleContextTest()
    {
        var options = new DbContextOptionsBuilder<DistributionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new DistributionDbContext(options, new ExplicitUserContext("test", null));

        var tenant  = new Tenant { Id = Guid.NewGuid(), Name = "TestTenant" };
        context.Tenants.Add(tenant);

        var product = new Product
        {
            Name = "TestProduct",
            Description = "Just a test",
            ProductType = ProductType.IagService
        };
        context.Products.Add(product);

        var customerPlugin = new Product
        {
            Name = "CustomerPlugin",
            Description = "Just a test plugin",
            ProductType = ProductType.CustomerExtension,
            DependsOnProductId = product.Id
        };
        context.Products.Add(customerPlugin);

        var productRelease = new Release
        {
            ProductId = product.Id,
            ReleaseVersion = "1.0.0",
            ReleaseDate = DateTime.Today
        };
        var productRelease101 = new Release
        {
            ProductId = product.Id,
            ReleaseVersion = "1.0.1",
            ReleaseDate = DateTime.Today
        };
        var pluginRelease = new Release
        {
            ProductId = customerPlugin.Id,
            ReleaseVersion = "1.0.0",
            ReleaseDate = DateTime.Today
        };
        context.Releases.Add(productRelease);
        context.Releases.Add(productRelease101);
        context.Releases.Add(pluginRelease);

        var baseDll = new FileStore
        {
            Name = "Base.dll",
            ProductVersion = "1.0.0",
            FileVersion = "1.0.0",
            Checksum = Encoding.UTF8.GetBytes("123"),
            Data = Encoding.UTF8.GetBytes("Content")
        };
        var productLogicADll = new FileStore
        {
            Name = "ProductLogicA.dll",
            ProductVersion = "1.0.0",
            FileVersion = "1.0.0",
            Checksum = Encoding.UTF8.GetBytes("123"),
            Data = Encoding.UTF8.GetBytes("Content")
        };
        var productLogicBDll = new FileStore
        {
            Name = "ProductLogicB.dll",
            ProductVersion = "1.0.0",
            FileVersion = "1.0.0",
            Checksum = Encoding.UTF8.GetBytes("123"),
            Data = Encoding.UTF8.GetBytes("Content")
        };
        var productLogicA101Dll = new FileStore
        {
            Name = "ProductLogicA.dll",
            ProductVersion = "1.0.1",
            FileVersion = "1.0.1",
            Checksum = Encoding.UTF8.GetBytes("123"),
            Data = Encoding.UTF8.GetBytes("Content")
        };
        var pluginLogicDll = new FileStore
        {
            Name = "PluginLogic.dll",
            ProductVersion = "1.0.0",
            FileVersion = "1.0.0",
            Checksum = Encoding.UTF8.GetBytes("123"),
            Data = Encoding.UTF8.GetBytes("Content")
        };
        context.FileStores.Add(baseDll);
        context.FileStores.Add(productLogicADll);
        context.FileStores.Add(productLogicBDll);
        context.FileStores.Add(productLogicA101Dll);
        context.FileStores.Add(pluginLogicDll);

        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = productRelease.Id, FileStoreId = baseDll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = productRelease.Id, FileStoreId = productLogicADll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = productRelease.Id, FileStoreId = productLogicBDll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = productRelease101.Id, FileStoreId = baseDll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = productRelease101.Id, FileStoreId = productLogicA101Dll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = productRelease101.Id, FileStoreId = productLogicBDll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = pluginRelease.Id, FileStoreId = baseDll.Id });
        context.ReleaseFileStores.Add(new ReleaseFileStore { ReleaseId = pluginRelease.Id, FileStoreId = pluginLogicDll.Id });

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Simplution Software GmbH"
        };
        context.Customers.Add(customer);
        context.ProductCustomers.Add(new ProductCustomer {ProductId = product.Id, CustomerId = customer.Id});

        var installation = new Installation
        {
            InstanceName = "TestInstallation",
            CustomerId = customer.Id,
            ProductId = productRelease101.ProductId
        };
        context.Installations.Add(installation);

        context.SaveChanges();

        Assert.NotEmpty(context.Products);
        Assert.NotEmpty(context.Releases);
        Assert.NotEmpty(context.ReleaseFileStores);
        Assert.NotEmpty(context.FileStores);
        Assert.NotEmpty(context.Customers);
        Assert.NotEmpty(context.ProductCustomers);
        Assert.All(context.Products, p => Assert.NotEmpty(p.Releases));
        Assert.All(context.Releases, r => Assert.NotEmpty(r.ReleaseFileStores));
        Assert.All(context.Customers, r => Assert.NotEmpty(r.ProductCustomers));
        Assert.Single(context.Products, p => p.DependsOnProductId.HasValue);
        Assert.Single(context.Products, p => !p.DependsOnProductId.HasValue);
        Assert.Single(context.Installations);
    }
}