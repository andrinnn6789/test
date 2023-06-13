using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.IAG.ControlCenter.Config;

namespace IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncCustomers;

public class SyncCustomersJobConfig : JobConfig<SyncCustomersJob>
{
    public BackendConfig Backend { get; set; } = new();
    public VinXConfig VinX { get; set; } = new();
}