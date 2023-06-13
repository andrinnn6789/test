using System.Composition;
using System.Diagnostics.CodeAnalysis;
using IAG.Common.EBill.BusinessLogic;
using IAG.Common.MailSender;
using IAG.Infrastructure.DI;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;
using IAG.VinX.SwissDrink.EBill.BusinessLogic.Implementation;
using IAG.VinX.SwissDrink.MailSender;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.VinX.SwissDrink.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddScoped<IAtlasConnectorEbill, AtlasConnectorEbillSwissDrink>();
        services.AddScoped<IMailEnhancer, MailEnhancerZugferd>();
        services.AddScoped<IReceiptCalculator, ReceiptCalculator>();
        services.AddScoped<IDdInvoiceAccess, DdInvoiceAccess>();
        services.AddScoped<IInvoiceClient, InvoiceClient>();
    }
}