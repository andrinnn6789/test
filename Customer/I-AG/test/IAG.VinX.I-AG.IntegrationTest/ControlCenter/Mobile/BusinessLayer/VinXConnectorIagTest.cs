using System;

using IAG.Common.TestHelper.Arrange;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Mobile.BusinessLayer;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Mobile.BusinessLayer;

public class VinXConnectorIagTest
{
    [Fact]
    public void TestMobileConfig()
    {
        // for coverage
        var config = new VinXConfig
        {
            ConnectionString = TestDbConnectionStrings.VinXTrunk
        };
        Assert.Equal(TestDbConnectionStrings.VinXTrunk, config.ConnectionString);
    }

    [Fact]
    public void TestUpdateLicence()
    {
        var sybaseConnection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
        var vinx = new VinXConnectorIag(sybaseConnection);
            
        vinx.UpdateLicence(new MobileLicence());
    }

    [Fact]
    public void TestGetLicenceConfig()
    {
        var sybaseConnection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
        var vinx = new VinXConnectorIag(sybaseConnection);

        var result =  vinx.GetLicenceConfig(DateTime.MinValue);
        Assert.NotNull(result);
    }

}