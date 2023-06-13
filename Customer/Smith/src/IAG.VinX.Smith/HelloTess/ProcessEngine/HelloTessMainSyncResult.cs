using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;

namespace IAG.VinX.Smith.HelloTess.ProcessEngine;

public class HelloTessMainSyncResult : JobResult
{
    public HelloTessMainSyncResult()
    {
        HelloTessSystemSyncResults = new List<HelloTessSystemSyncResult>();
    }

    public List<HelloTessSystemSyncResult> HelloTessSystemSyncResults { get; set; }
}