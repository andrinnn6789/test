using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.Smith.SalesPdf.ProcessEngine;

public class ExtractorJobResult : JobResult
{
    public ExtractorJobResult()
    {
        SyncResults = new List<SyncResult>();
    }

    public readonly List<SyncResult> SyncResults;
}