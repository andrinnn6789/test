using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

public class ExportBaseDataConfig : JobConfig<ExportBaseDataJob>
{
    public ExportBaseDataConfig()
    {
        Config = new ExportBaseConfig();
    }

    public ExportBaseConfig Config { get; set; }
}