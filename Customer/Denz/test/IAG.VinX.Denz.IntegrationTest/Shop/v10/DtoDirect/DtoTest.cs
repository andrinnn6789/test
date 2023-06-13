using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Denz.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Denz.IntegrationTest.Shop.v10.DtoDirect;

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
    public void ArticleDwa()
    {
        _ = _connection.GetQueryable<ArticleDwa>().FirstOrDefault();
    }

    [Fact]
    public void WineDwa()
    {
        _ = _connection.GetQueryable<WineDwa>().FirstOrDefault();
    }
}