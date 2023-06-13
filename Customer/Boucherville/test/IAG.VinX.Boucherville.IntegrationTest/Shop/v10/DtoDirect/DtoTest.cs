using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Boucherville.Shop.v10.DtoDirect;

using Xunit;

namespace IAG.VinX.Boucherville.IntegrationTest.Shop.v10.DtoDirect;

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
    public void AddressBov()
    {
        _ = _connection.GetQueryable<AddressBov>().FirstOrDefault();
    }

    [Fact]
    public void ArticleBov()
    {
        _ = _connection.GetQueryable<ArticleBov>().FirstOrDefault();
    }

    [Fact]
    public void CountryBov()
    {
        _ = _connection.GetQueryable<CountryBov>().FirstOrDefault();
    }

    [Fact]
    public void DeliveryConditionBov()
    {
        _ = _connection.GetQueryable<DeliveryConditionBov>().FirstOrDefault();
    }

    [Fact]
    public void ECommerceGroupBov()
    {
        _ = _connection.GetQueryable<ECommerceGroupBov>().FirstOrDefault();
    }

    [Fact]
    public void FillingBov()
    {
        _ = _connection.GetQueryable<FillingBov>().FirstOrDefault();
    }

    [Fact]
    public void GrapeBov()
    {
        _ = _connection.GetQueryable<GrapeBov>().FirstOrDefault();
    }

    [Fact]
    public void PaymentConditionBov()
    {
        _ = _connection.GetQueryable<PaymentConditionBov>().FirstOrDefault();
    }

    [Fact]
    public void PredicateBov()
    {
        _ = _connection.GetQueryable<PredicateBov>().FirstOrDefault();
    }

    [Fact]
    public void ProducerBov()
    {
        _ = _connection.GetQueryable<ProducerBov>().FirstOrDefault();
    }

    [Fact]
    public void ReceiptTypeBov()
    {
        _ = _connection.GetQueryable<ReceiptTypeBov>().FirstOrDefault();
    }

    [Fact]
    public void RegionBov()
    {
        _ = _connection.GetQueryable<RegionBov>().FirstOrDefault();
    }

    [Fact]
    public void SelectionCodeBov()
    {
        _ = _connection.GetQueryable<SelectionCodeBov>().FirstOrDefault();
    }

    [Fact]
    public void TradingUnitBov()
    {
        _ = _connection.GetQueryable<TradingUnitBov>().FirstOrDefault();
    }

    [Fact]
    public void WineBov()
    {
        _ = _connection.GetQueryable<WineBov>().FirstOrDefault();
    }
}