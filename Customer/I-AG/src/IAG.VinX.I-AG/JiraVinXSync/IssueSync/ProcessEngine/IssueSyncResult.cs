using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.ProcessEngine;

public class IssueSyncResult : JobResult
{
    public int CreatedPendenzenCount { get; set; }

    public int UpdatedPendenzenCount { get; set; }
}