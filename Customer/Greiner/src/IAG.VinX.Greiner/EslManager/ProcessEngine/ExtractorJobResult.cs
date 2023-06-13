using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.Greiner.EslManager.ProcessEngine;

public class ExtractorJobResult : JobResult
{
    public int ExportCount { get; set; }
    public int SuccessCount { get; set; }
}