using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.ProcessEngine;

namespace IAG.VinX.SwissDrink.Resource;

internal static class ResourceIds
{
    private const string ResourcePrefix = "VinX.SwissDrink.";

    // Jobs
    private const string ResourcePrefixJob = ResourcePrefix + "Job.";
    public const string ResourcePrefixJobDd = ResourcePrefixJob + "DigitalDrink.";
    private const string ReceiveInvoicesSdlJobName = ReceiveInvoicesSdlJob.JobName;

    public const string ReceiveInvoicesSdlFailed = ReceiveInvoicesSdlJobName + "Failed to receive invoices: {0}";
    public const string CreateInvoiceSdlFailed = ReceiveInvoicesSdlJobName + "Failed to create invoice: {0}";
    public const string ProcessInvoiceInfo = ReceiveInvoicesSdlJobName + "Process invoice: {0}";
    public const string InvoiceSkipped = ReceiveInvoicesSdlJobName + "invoice {0} skipped";
}