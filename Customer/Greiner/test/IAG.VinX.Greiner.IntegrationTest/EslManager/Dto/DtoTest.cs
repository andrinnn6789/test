using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Greiner.EslManager.Dto;

using Xunit;

namespace IAG.VinX.Greiner.IntegrationTest.EslManager.Dto;

public class DtoTest
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
    }

    [Fact]
    public void ArticleDtoTest()
    {
        Assert.Equal(5, _connection.GetQueryable<Article>().Take(5).ToList().Count);
    }

    [Fact]
    public void GtinGroupDtoTest()
    {
        Assert.Equal(5, _connection.GetQueryable<GtinGroup>().Take(5).ToList().Count);
    }

    [Fact]
    public void ArticleDelDtoTest()
    {
        Assert.Single(_connection.GetQueryable<ArticleDel>().Take(1).ToList());
    }
}