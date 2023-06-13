using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Chardonnens.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Chardonnens.IntegrationTest.Shop.v10.DtoDirect;

public class DtoTest : IDisposable
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    [Fact]
    public void ArticleCefs()
    {
        _ = _connection.GetQueryable<ArticleCefs>().FirstOrDefault();
    }
}