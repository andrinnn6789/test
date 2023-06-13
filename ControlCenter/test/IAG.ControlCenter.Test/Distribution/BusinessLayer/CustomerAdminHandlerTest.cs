using System;
using System.Linq;
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

public class CustomerAdminHandlerTest : IDisposable
{
    private readonly DistributionDbContext _context;
    private readonly CustomerAdminHandler _handler;
    private readonly Guid _testProductId;

    public CustomerAdminHandlerTest()
    {
        var options = new DbContextOptionsBuilder<DistributionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new DistributionDbContext(options, new ExplicitUserContext("test", null));
        _handler = new CustomerAdminHandler(_context);

        var testProduct = new Product() {Name = "TestProduct"};
        _context.Products.Add(testProduct);
        _context.SaveChanges();
        _testProductId = testProduct.Id;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task RegisterAndGetCustomersTest()
    {
        var customerId = Guid.NewGuid();
        var customerRegistration = new CustomerRegistration()
        {
            CustomerId = customerId,
            CustomerName = "Alpha Company",
            CustomerCategoryId = 42,
            Description = "TestDescription"
        };
        var customer = await _handler.RegisterCustomerAsync(customerRegistration);
        var customers = (await _handler.GetCustomersAsync()).ToList();

        Assert.NotNull(customer);
        Assert.Equal(customerId, customer.Id);
        Assert.Equal("Alpha Company", customer.CustomerName);
        Assert.Equal(42, customer.CustomerCategoryId);
        Assert.Equal("TestDescription", customer.Description);
        Assert.Empty(customer.ProductIds);
        Assert.NotNull(customers);
        Assert.Equal(customer.Id, Assert.Single(customers)?.Id);
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterCustomerAsync(null));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterCustomerAsync(new CustomerRegistration()));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterCustomerAsync(new CustomerRegistration() { CustomerName = string.Empty }));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RegisterCustomerAsync(new CustomerRegistration() { CustomerName = "NotEmpty", CustomerId = Guid.Empty}));
    }

    [Fact]
    public async Task AddAndRemoveProductsTest()
    {
        var customerId = Guid.NewGuid();
        var customerRegistration = new CustomerRegistration()
        {
            CustomerId = customerId,
            CustomerName = "Beta Company"
        };
        var customer = await _handler.RegisterCustomerAsync(customerRegistration);

        await _handler.AddProductsAsync(customer.Id, new[] {_testProductId});
        var customerWithProduct = (await _handler.GetCustomersAsync()).FirstOrDefault();
        await _handler.RemoveProductsAsync(customer.Id, new[] { _testProductId });
        var customerWithRemovedProduct = (await _handler.GetCustomersAsync()).FirstOrDefault();

        Assert.NotNull(customerWithProduct);
        Assert.Equal(_testProductId, Assert.Single(customerWithProduct.ProductIds));
        Assert.NotNull(customerWithRemovedProduct);
        Assert.Empty(customerWithRemovedProduct.ProductIds);
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.AddProductsAsync(Guid.Empty, Enumerable.Empty<Guid>()));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.AddProductsAsync(customer.Id, null));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RemoveProductsAsync(Guid.Empty, Enumerable.Empty<Guid>()));
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.RemoveProductsAsync(customer.Id, null));
    }
}