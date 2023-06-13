using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

public class GastivoImportJobResult : JobResult
{
    public int ImportedCount { get; set; }
}