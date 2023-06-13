using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.CDV.Wamas.Common.Config;

namespace IAG.VinX.CDV.Wamas.Common.ProcessEngine;

public class WamasBaseJobConfig<T> : JobConfig<T>
{
    public WamasFtpConfig WamasFtpConfig { get; set; } = new();

    public string ConnectionString { get; set; } = "$$sybaseConnection$";
}