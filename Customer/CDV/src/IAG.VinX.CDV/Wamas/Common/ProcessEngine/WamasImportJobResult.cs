using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.CDV.Wamas.Common.ProcessEngine;

public class WamasImportJobResult : JobResult
{
    public int ImportedCount { get; set; }
}