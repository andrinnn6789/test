using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Smith.HelloTess.VinX;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.HelloTess.VinX;

public class VxClientTest
{
    private readonly ISybaseConnectionFactory _factory;

    public VxClientTest()
    {
        _factory = SybaseConnectionFactoryHelper.CreateFactory();
    }

    [Fact]
    public void GetArtikelTest()
    {
        var client = new VxArticleClient(_factory.CreateConnection());
        client.SetFilter(ConfigHelper.SmithAndSmithSystemConfig.PriceGroupForProdCost, ConfigHelper.SmithAndSmithSystemConfig.CustomerForProdCost);
        var _ = client.Get();
    }

    [Fact]
    public void GetArtikelkategorieTest()
    {
        var _ = new VxArticleCategoryClient(_factory.CreateConnection()).Get();
    }
}