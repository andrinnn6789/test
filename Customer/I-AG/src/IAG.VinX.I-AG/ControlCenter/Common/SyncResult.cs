using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.IAG.ControlCenter.Common;

public class SyncResult : JobResult
{ 
    public string SyncName { get; set; }
    public int SuccessCount { get; set; }
}