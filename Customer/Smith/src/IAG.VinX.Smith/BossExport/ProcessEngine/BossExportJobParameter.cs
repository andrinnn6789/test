using IAG.Infrastructure.ProcessEngine.JobModel;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.BossExport.ProcessEngine;

[UsedImplicitly]
public class BossExportJobParameter : JobParameter
{
    public bool ExportAll { get; set; }
    public bool UpdateExported { get; set; } = true;
}