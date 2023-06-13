using System;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.InstallClient.Models.Mapper;

using Xunit;

namespace IAG.InstallClient.Test.Models.Mapper;

public class CustomerViewModelMapperTest
{
    [Fact]
    public void MapTest()
    {
        var mapper = new CustomerViewModelMapper();
        var testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };
        var model = mapper.NewDestination(testCustomer);

        Assert.Equal(testCustomer.Id, model.CustomerId);
        Assert.Equal(testCustomer.CustomerName, model.CustomerName);
        Assert.Equal(testCustomer.Description, model.Description);
    }
}