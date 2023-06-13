using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

public class ExportResultDetail
{
    public string Name { get; set; }
    public int ExportCount { get; set; }
}

public class ExportResult : JobResult
{
    public ExportResult()
    {
        SyncResult = new List<ExportResultDetail>();
    }

    public string Name { get; set; }

    public List<ExportResultDetail> SyncResult { get; }
}