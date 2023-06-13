using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.DataAccess;
using IAG.VinX.Basket.Dto;

namespace IAG.VinX.Globalwine.ShopNext.DataAccess;

public class StockCalculatorGw : StockCalculator
{
    public StockCalculatorGw(ISybaseConnection connection): base(connection)
    {
    }
    public override IQueryable<StockData> GetStock(List<ArticleParameter> articleParameters)
    {
        var stocks = base.GetStock(articleParameters);
        return stocks.Where(s => s.WarehouseId == 91);
    }
}