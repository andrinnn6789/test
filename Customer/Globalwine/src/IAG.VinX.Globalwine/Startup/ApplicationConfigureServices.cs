using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.VinX.Basket.BusinessLogic;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.ShopNext.BusinessLogic;
using IAG.VinX.Globalwine.ShopNext.DataAccess;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.VinX.Globalwine.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddScoped<IOnlineClient<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>, 
            OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>>();
        services.AddScoped<IBasketCalculator<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>, 
            BasketCalculator<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>>();
        services.AddScoped<IShippingCostCalculator, ShippingCostCalculatorGw>();
        services.AddScoped<IStockCalculator, StockCalculatorGw>();
    }
}