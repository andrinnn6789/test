using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.VinX.Smith.SalesPdf.ProcessEngine;

public class ExtractorJobConfig<TJob> : JobConfig<TJob>
{
    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public ExtractorWodConfig ExtractorWodConfig { get; set; } = new();
}