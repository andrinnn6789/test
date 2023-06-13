using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

public class ExportStockConfig : JobConfig<ExportStockJob>
{
    public ExportStockConfig()
    {
        Config = new ExportBaseConfig();
    }

    public ExportBaseConfig Config { get; set; }
}