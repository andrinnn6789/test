using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.IAG.ControlCenter.Config;

namespace IAG.VinX.IAG.ControlCenter.Mobile.ProcessEngine;

public class LicenceSyncerConfig : JobConfig<LicenceSyncerJob>
{
    public LicenceSyncerConfig()
    {
        VinXConfig = new VinXConfig();
        BackendConfig = new BackendConfig();
    }

    public VinXConfig VinXConfig { get; set; }

    public BackendConfig BackendConfig { get; set; }

    public int DiffSyncsPerFullSync { get; set; } = 1;
}