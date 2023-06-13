using IAG.VinX.Zweifel.MySign.ProcessEngine.Export;
using IAG.VinX.Zweifel.MySign.ProcessEngine.Import;

namespace IAG.VinX.Zweifel.Resource;

public static class ResourceIds
{
    // jobs
    public const string ResourcePrefixJob = "Zweifel.Job.";
    internal const string ExportBaseDataJobName = ExportBaseDataJob.JobName;
    internal const string ExportCustomerJobName = ExportCustomerJob.JobName;
    internal const string ExportStockJobName = ExportStockJob.JobName;
    internal const string ImportJobName = ImportJob.JobName;
        
    internal const string OrderBravoErrors = ResourcePrefixJob + "OrderBravo.";
    internal const string ObInsertOrderError = OrderBravoErrors + "Error inserting order {0}: {1}";
    internal const string ObExportErrorMappingArticle = OrderBravoErrors + "Error while mapping article {0} to Bravo Order, {1}";
}