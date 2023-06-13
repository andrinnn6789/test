using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

public class GastivoExportJobResult : JobResult
{
    public int ExportedCount { get; set; }
}