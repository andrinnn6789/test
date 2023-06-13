using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Import;

public class ImportResultDetail
{
    public string Name { get; set; }
    public int RecordCount { get; set; }
    public int InsertCount { get; set; }
    public int UpdateCount { get; set; }
    public int ErrorCount { get; set; }
}

public class ImportResult : JobResult
{
    public ImportResult()
    {
        SyncResult = new List<ImportResultDetail>();
    }

    public string Name { get; set; }

    public List<ImportResultDetail> SyncResult { get; }
}