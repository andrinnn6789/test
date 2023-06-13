using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

public class ExportCustomerConfig : JobConfig<ExportCustomerJob>
{
    public ExportCustomerConfig()
    {
        Config = new ExportBaseConfig
        {
            ExportFolder = "$$exchangeRoot$\\Export\\Kunden"
        };
    }

    public ExportBaseConfig Config { get; set; }
}