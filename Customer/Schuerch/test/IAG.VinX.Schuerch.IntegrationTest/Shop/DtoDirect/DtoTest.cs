using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Schuerch.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Schuerch.IntegrationTest.Shop.DtoDirect;

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
    public void FillingSchuerch()
    {
        _ = _connection.GetQueryable<FillingSchuerch>().FirstOrDefault();
    }

    [Fact]
    public void ArticleSchuerch()
    {
        _ = _connection.GetQueryable<ArticleSchuerch>().FirstOrDefault();
    }
}