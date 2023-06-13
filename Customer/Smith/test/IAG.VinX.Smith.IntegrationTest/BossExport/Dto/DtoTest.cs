using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Smith.BossExport.Dto;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.BossExport.Dto;

public class DtoTest
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
    }

    [Fact]
    public void AddressTest()
    {
        var items = _connection.GetQueryable<ArticleBoss>().Take(1).ToList();
        Assert.NotEmpty(items);
    }
}