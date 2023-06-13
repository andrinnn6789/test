using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.CDV.Wamas.Common.ProcessEngine;

public class WamasExportJobResult : JobResult
{
    public int ExportedCount { get; set; }
}