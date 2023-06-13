using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.Smith.BossExport.ProcessEngine;

public class BossExportJobResult : JobResult
{
    public int ExportCount { get; set; }
}