// 
// 2021 03 31 11:11

using IAG.VinX.Schüwo.SV.Config;

namespace IAG.VinX.Schüwo.SV.ProcessEngine;

public interface ISvBaseJobConfig
{
    string VinXConnectionString { get; set; }
    FtpEndpointConfig FtpEndpointConfig { get; set; }
    FtpPathConfig FtpPathConfig { get; set; }
}