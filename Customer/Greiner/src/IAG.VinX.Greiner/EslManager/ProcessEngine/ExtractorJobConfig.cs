using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.Greiner.EslManager.Config;

namespace IAG.VinX.Greiner.EslManager.ProcessEngine;

public class ExtractorJobConfig : JobConfig<ExtractorJob>
{
    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";
    public EslExportConfig EslExportConfig { get; set; } = new();
}