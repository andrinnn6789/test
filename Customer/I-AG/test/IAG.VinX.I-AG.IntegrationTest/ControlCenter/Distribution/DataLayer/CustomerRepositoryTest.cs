using System;
using System.Linq;

using IAG.Common.TestHelper.Arrange;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.DataLayer;

public class CustomerRepositoryTest
{
    [Fact]
    public void GetCustomersTest()
    {
        var connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
        var repository = new CustomerRepository(connection);

        var customers = repository.GetCustomers()?.ToList();

        Assert.NotNull(customers);
        Assert.Contains(customers, c => c.UsesVinX);
        Assert.Contains(customers, c => c.UsesPerformX);
        Assert.All(customers, c => Assert.NotEqual(Guid.Empty, c.CustomerId));
        Assert.All(customers, c => Assert.NotEmpty(c.CustomerName));
        Assert.All(customers, c => Assert.NotEmpty(c.Description));
        Assert.All(customers, c => Assert.True(c.UsesPerformX || c.UsesVinX));
    }
}