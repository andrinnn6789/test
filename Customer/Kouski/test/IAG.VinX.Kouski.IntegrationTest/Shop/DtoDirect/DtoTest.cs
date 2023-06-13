using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Kouski.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Kouski.Test.Shop.DtoDirect;

public class DtoTest : IDisposable
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void ArticleCefs()
    {
        _ = _connection.GetQueryable<ArticleKouski>().FirstOrDefault();
    }
}