using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.Models.Mapper;

using Xunit;

namespace IAG.InstallClient.Test.Models.Mapper;

public class InstallationViewModelMapperTest
{
    [Fact]
    public void MapTest()
    {
        var mapper = new InstallationViewModelMapper();
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = "TestProduct",
            Version = "TestVersion",
            CustomerPluginName = "TestCustomerPluginName"
        };
        var model = mapper.NewDestination(testInstallation);

        Assert.Equal(testInstallation.InstanceName, model.InstanceName);
        Assert.Equal(testInstallation.ProductName, model.ProductName);
        Assert.Equal(testInstallation.Version, model.Version);
        Assert.Equal(testInstallation.CustomerPluginName, model.CustomerPluginName);
    }
}