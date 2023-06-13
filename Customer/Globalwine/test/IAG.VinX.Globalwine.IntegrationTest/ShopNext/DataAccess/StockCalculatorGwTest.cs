using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Globalwine.ShopNext.DataAccess;

using Moq;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.DataAccess;

public class StockCalculatorGwTest
{
    private readonly Mock<ISybaseConnection> _connection;
    private readonly StockCalculatorGw _stockCalculator;

    public StockCalculatorGwTest()
    {
        _connection = SybaseConnectionFactoryHelper.CreateConnectioMock();
        _stockCalculator = new StockCalculatorGw(_connection.Object);
    }

    [Fact]
    public void GetStock()
    {
        _connection.Setup(m => m.GetQueryable<StockData>()).Returns(new List<StockData>
            {
                new()
                {
                    ArticleId = 1,
                    WarehouseId = 91,
                    QuantityAvailable = 1
                },
                new()
                {
                    ArticleId = 1,
                    WarehouseId = 91,
                    QuantityAvailable = 1,
                    ForShop = true
                },
                new()
                {
                    ArticleId = 1,
                    WarehouseId = 92,
                    QuantityAvailable = 1
                },
                new()
                {
                    ArticleId = 2,
                    WarehouseId = 91,
                    QuantityAvailable = 2
                }
            }
            .AsQueryable);
        var stocks = _stockCalculator.GetStock(
            new List<ArticleParameter>
            {
                new()
                {
                    ArticleId = 1
                }
            });
        Assert.Equal(1, stocks.Count());
        Assert.Equal(1, stocks.First().QuantityAvailable);
    }
}