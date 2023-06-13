using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;

namespace IAG.VinX.Smith.HelloTess.ProcessEngine;

public class HelloTessMainSyncConfig : JobConfig<HelloTessMainSyncJob>
{
    public HelloTessMainSyncConfig()
    {
        HelloTessSystemConfigs = new List<HelloTessSystemConfig>();
        SyncSystemDefaults = new SyncSystemDefaults();
    }

    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public List<HelloTessSystemConfig> HelloTessSystemConfigs { get; set; }

    public SyncSystemDefaults SyncSystemDefaults { get; set; }
}