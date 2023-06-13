using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Gedex.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Gedex.IntegrationTest.Shop.v10.DtoDirect;

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
    public void ArticleGga()
    {
        _ = _connection.GetQueryable<ArticleGga>().FirstOrDefault();
    }

    [Fact]
    public void GrapeAmountGga()
    {
        _ = _connection.GetQueryable<GrapeAmountGga>().FirstOrDefault();
    }

    [Fact]
    public void MatchGga()
    {
        _ = _connection.GetQueryable<MatchGga>().FirstOrDefault();
    }

    [Fact]
    public void StrengthGga()
    {
        _ = _connection.GetQueryable<StrengthGga>().FirstOrDefault();
    }

    [Fact]
    public void TasteGga()
    {
        _ = _connection.GetQueryable<TasteGga>().FirstOrDefault();
    }

    [Fact]
    public void WineGga()
    {
        _ = _connection.GetQueryable<WineGga>().FirstOrDefault();
    }

    [Fact]
    public void WineMatchRelationGga()
    {
        _ = _connection.GetQueryable<WineMatchRelationGga>().FirstOrDefault();
    }

    [Fact]
    public void WineTasteRelationGga()
    {
        _ = _connection.GetQueryable<WineTasteRelationGga>().FirstOrDefault();
    }
}