using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

public class CustomerSyncLogicTest
{
    private readonly CustomerSyncLogic _logic;
    private readonly Guid _productIdVinX = Guid.NewGuid();
    private readonly Guid _productIdPerformX = Guid.NewGuid();
    private readonly Guid _productIdInstallerTool = Guid.NewGuid();
    private readonly Mock<ICustomerAdminClient> _customerAdminClientMock;

    private readonly List<CustomerInfo> _alreadyRegisteredCustomers = new();
    private readonly List<IagCustomer> _customers = new();
    private readonly Mock<IMessageLogger> _loggerMock;

    public CustomerSyncLogicTest()
    {
        _customerAdminClientMock = new Mock<ICustomerAdminClient>();
        _customerAdminClientMock.Setup(m => m.GetCustomersAsync()).ReturnsAsync(_alreadyRegisteredCustomers);

        var productAdminClientMock = new Mock<IProductAdminClient>();
        productAdminClientMock.Setup(m => m.RegisterProductAsync("VinX", ProductType.IagService, null))
            .ReturnsAsync(new ProductInfo { Id = _productIdVinX});
        productAdminClientMock.Setup(m => m.RegisterProductAsync("PerformX", ProductType.IagService, null))
            .ReturnsAsync(new ProductInfo { Id = _productIdPerformX });
        productAdminClientMock.Setup(m => m.RegisterProductAsync("Installer", ProductType.Updater, null))
            .ReturnsAsync(new ProductInfo { Id = _productIdInstallerTool });

        var customerRepositoryMock = new Mock<ICustomerRepository>();
        customerRepositoryMock.Setup(m => m.GetCustomers()).Returns(_customers);

        _loggerMock = new Mock<IMessageLogger>();

        _logic = new CustomerSyncLogic(_customerAdminClientMock.Object, productAdminClientMock.Object, customerRepositoryMock.Object, _loggerMock.Object, new SyncResult());
    }

    [Fact]
    public async Task HappyPathTest()
    {
        var registeredCustomers = new Dictionary<Guid, string>();
        var registeredProducts = new Dictionary<Guid, List<Guid>>();

        _customerAdminClientMock.Setup(m => m.RegisterCustomerAsync(It.IsAny<Guid>() ,It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((Guid id, string name, int _, string _) =>
            {
                registeredCustomers.Add(id, name);
                return new CustomerInfo { Id = id };
            });
        _customerAdminClientMock.Setup(m => m.AddProductsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()))
            .Callback((Guid customerId, IEnumerable<Guid> productIds) =>
            {
                registeredProducts.Add(customerId, productIds.ToList());
            });

        var vinXCustomerId = Guid.NewGuid();
        var performXCustomerId = Guid.NewGuid();
        var bothXCustomerId = Guid.NewGuid();
        var existingCustomerId = Guid.NewGuid();

        _customers.Add(new IagCustomer { CustomerId = vinXCustomerId, CustomerName = "VinXCustomer", UsesVinX = true, Description = "Test"} );
        _customers.Add(new IagCustomer { CustomerId = performXCustomerId, CustomerName = "PerformXCustomer", UsesPerformX = true });
        _customers.Add(new IagCustomer { CustomerId = bothXCustomerId, CustomerName = "BothXCustomer", UsesVinX = true, UsesPerformX = true });
        _customers.Add(new IagCustomer { CustomerId = existingCustomerId, CustomerName = "ExistingCustomer" });
        _alreadyRegisteredCustomers.Add(new CustomerInfo { Id = existingCustomerId });

        await _logic.DoSyncAsync(new Mock<IJobHeartbeatObserver>().Object);

        Assert.Equal(3, registeredCustomers.Count);
        Assert.Equal(3, registeredProducts.Count);
        Assert.True(registeredCustomers.ContainsKey(vinXCustomerId));
        Assert.True(registeredCustomers.ContainsKey(performXCustomerId));
        Assert.True(registeredCustomers.ContainsKey(bothXCustomerId));
        Assert.False(registeredCustomers.ContainsKey(existingCustomerId));
        Assert.Equal("VinXCustomer", registeredCustomers[vinXCustomerId]);
        Assert.Equal("PerformXCustomer", registeredCustomers[performXCustomerId]);
        Assert.Equal("BothXCustomer", registeredCustomers[bothXCustomerId]);
        Assert.True(registeredProducts.ContainsKey(vinXCustomerId));
        Assert.True(registeredProducts.ContainsKey(performXCustomerId));
        Assert.True(registeredProducts.ContainsKey(bothXCustomerId));
        Assert.False(registeredProducts.ContainsKey(existingCustomerId));
        Assert.Contains(_productIdVinX, registeredProducts[vinXCustomerId]);
        Assert.Contains(_productIdPerformX, registeredProducts[performXCustomerId]);
        Assert.Contains(_productIdVinX, registeredProducts[bothXCustomerId]);
        Assert.Contains(_productIdPerformX, registeredProducts[bothXCustomerId]);
        Assert.All(registeredProducts, x => Assert.Contains(_productIdInstallerTool, x.Value));
        Assert.Equal(2, registeredProducts[vinXCustomerId].Count);
        Assert.Equal(2, registeredProducts[performXCustomerId].Count);
        Assert.Equal(3, registeredProducts[bothXCustomerId].Count);
    }

    [Fact]
    public async Task RegisterCustomerFailsTest()
    {
        _customerAdminClientMock.Setup(m => m.RegisterCustomerAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new LocalizableException("Nope"));

        var exceptionLogged = false;
        _loggerMock.Setup(m => m.AddMessage(It.IsAny<MessageTypeEnum>(), It.IsAny<string>(), It.IsAny<object[]>())).Callback<MessageTypeEnum, string, object[]>((type, _, _) =>
        {
            exceptionLogged = exceptionLogged || (type == MessageTypeEnum.Error);
        });

        _customers.Add(new IagCustomer { CustomerName = "TestCustomer", UsesPerformX = true, UsesVinX = true });

        await _logic.DoSyncAsync(new Mock<IJobHeartbeatObserver>().Object);

        Assert.True(exceptionLogged);
    }
}