using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.CDV.Gastivo.Common.Config;

namespace IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

public class GastivoBaseJobConfig<T> : JobConfig<T>
{
    public GastivoFtpConfig GastivoFtpConfig { get; set; } = new();

    public string ConnectionString { get; set; } = "$$sybaseConnection$";
}