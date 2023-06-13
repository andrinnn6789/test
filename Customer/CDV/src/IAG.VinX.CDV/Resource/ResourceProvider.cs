using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // jobs
        AddTemplate(ResourceIds.WamasPartnerExportJobName, "de", "WAMAS Partner-Export");
        AddTemplate(ResourceIds.WamasArticleExportJobName, "de", "WAMAS Artikel-Export");
        AddTemplate(ResourceIds.WamasPickListExportJobName, "de", "WAMAS Rüstschein-Export");
        AddTemplate(ResourceIds.WamasPurchaseOrderExportJobName, "de", "WAMAS Bestellungs-Export");
        AddTemplate(ResourceIds.WamasGoodsIssueImportJobName, "de", "WAMAS Warenausgangs-Import");
        AddTemplate(ResourceIds.WamasGoodsReceiptImportJobName, "de", "WAMAS Wareneingangs-Import");
        AddTemplate(ResourceIds.WamasStockAdjustmentImportJobName, "de", "WAMAS Bestandskorrektur-Import");
        AddTemplate(ResourceIds.WamasStockReportImportJobName, "de", "WAMAS Bestandsbericht-Import");
    }
}