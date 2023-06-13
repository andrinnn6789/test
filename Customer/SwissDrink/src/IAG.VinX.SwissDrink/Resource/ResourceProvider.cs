using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.SwissDrink.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // ReceiveInvoicesSdlJob
        AddTemplate(ResourceIds.ReceiveInvoicesSdlFailed, "en", "Failed to receive invoices: {0}");
        AddTemplate(ResourceIds.CreateInvoiceSdlFailed, "en", "Failed to create invoice: {0}");
        AddTemplate(ResourceIds.ProcessInvoiceInfo, "en", "Process invoice: {0}");
    }
}