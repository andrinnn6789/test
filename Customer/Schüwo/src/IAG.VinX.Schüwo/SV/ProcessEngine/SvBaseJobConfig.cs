using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.VinX.Schüwo.SV.Config;

namespace IAG.VinX.Schüwo.SV.ProcessEngine;

public class SvBaseJobConfig<T> : JobConfig<T>, ISvBaseJobConfig 
{
    public string VinXConnectionString { get; set; } = "$$sybaseConnection$";

    public FtpEndpointConfig FtpEndpointConfig { get; set; } = new();

    public FtpPathConfig FtpPathConfig { get; set; } = new();
}