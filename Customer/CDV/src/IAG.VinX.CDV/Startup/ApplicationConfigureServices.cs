using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PartnerExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.StockReportImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.VinX.CDV.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        // Wamas
        services.AddScoped<IPartnerExporter, PartnerExporter>();
        services.AddScoped<Wamas.ArticleExport.BusinessLogic.IArticleExporter, Wamas.ArticleExport.BusinessLogic.ArticleExporter>();
        services.AddScoped<IPurchaseOrderExporter, PurchaseOrderExporter>();
        services.AddScoped<IGoodsReceiptImporter, GoodsReceiptImporter>();
        services.AddScoped<IPickListExporter, PickListExporter>();
        services.AddScoped<IGoodsIssueImporter, GoodsIssueImporter>();
        services.AddScoped<IStockAdjustmentImporter, StockAdjustmentImporter>();
        services.AddScoped<IStockReportImporter, StockReportImporter>();
        services.AddScoped<Wamas.Common.DataAccess.IFtpConnector, Wamas.Common.DataAccess.FtpConnector>();
        
        // Gastivo
        services.AddScoped<ICustomerExporter, CustomerExporter>();
        services.AddScoped<Gastivo.ArticleExport.BusinessLogic.IArticleExporter, Gastivo.ArticleExport.BusinessLogic.ArticleExporter>();
        services.AddScoped<IPriceExporter, PriceExporter>();
        services.AddScoped<IOrderImporter, OrderImporter>();
        services.AddScoped<Gastivo.Common.Ftp.IFtpConnector, Gastivo.Common.Ftp.FtpConnector>();
    }
}