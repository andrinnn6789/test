using IAG.VinX.ExternalDataProvider.ProcessEngine.Common;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.ProcessEngine;

public class ReceiveInvoicesSdlJobConfig : CommonJobConfig<ReceiveInvoicesSdlJob>
{
    public ReceiveInvoicesSdlJobConfig()
    {
        CronExpression = "*/15 * * * *"; // every 15 minutes
    }

    public int AddressIdZfv { get; set; } = 210;
}