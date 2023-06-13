using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.VinX.OrderBravo.BusinessLogic.Interface;
using IAG.VinX.OrderBravo.Dto.Interfaces;
using IAG.VinX.Zweifel.OrderBravo.BusinessLogic;
using IAG.VinX.Zweifel.OrderBravo.Dto;
using IAG.VinX.Zweifel.S1M.BusinessLogic;
using IAG.VinX.Zweifel.S1M.Sybase;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.VinX.Zweifel.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        // Order Bravo
        services.AddScoped<IObArticle, ObzArticle>();
        services.AddScoped<IObsCatalogMapper, ObzCatalogMapper>();
        // S1M
        services.AddScoped<IDeliveryClient, DeliveryClient>();
        services.AddScoped<IS1MDeliveryComposer, S1MDeliveryComposer>();
        services.AddScoped<IS1MMediaWriter, S1MMediaWriter>();
    }
}