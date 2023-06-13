using System;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

public class CustomerManagerTest
{
    [Fact]
    public async Task NoCcBaseUrlConfigTest()
    {
        var customerManager = new CustomerManager(new Mock<IConfiguration>().Object);

        await Assert.ThrowsAsync<LocalizableException>(() => customerManager.GetCustomerInformationAsync(Guid.Empty));
    }

    [Fact]
    public async Task GetAndSetCurrentCustomerTest()
    {
        var customerManager = new CustomerManager(new Mock<IConfiguration>().Object);

        var testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };
        var customerBeforeSet = await customerManager.GetCurrentCustomerInformationAsync();
        await customerManager.SetCurrentCustomerInformationAsync(testCustomer);
        var customerAfterSet = await customerManager.GetCurrentCustomerInformationAsync();
        await customerManager.SetCurrentCustomerInformationAsync(null);
        var customerAfterUnset = await customerManager.GetCurrentCustomerInformationAsync();

        Assert.Null(customerBeforeSet);
        Assert.NotNull(customerAfterSet);
        Assert.Equal(testCustomer.Id, customerAfterSet.Id);
        Assert.Equal(testCustomer.CustomerName, customerAfterSet.CustomerName);
        Assert.Equal(testCustomer.Description, customerAfterSet.Description);
        Assert.Null(customerAfterUnset);
    }
}