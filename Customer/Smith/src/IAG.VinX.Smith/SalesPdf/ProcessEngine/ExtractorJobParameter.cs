using IAG.Infrastructure.ProcessEngine.JobModel;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.SalesPdf.ProcessEngine;

[UsedImplicitly]
public class ExtractorJobParameter : JobParameter
{
    public bool RebuildAll { get; set; }
}