using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.Config;

[ExcludeFromCodeCoverage]
public class GastivoFtpConfig
{
    public string Url { get; set; } = "$$gastivoFtpUrl$";

    public string User { get; set; } = "$$gastivoFtpUser$";

    public string Password { get; set; } = "$$gastivoFtpPassword$";

    public string ImportDir { get; set; } = "/OUTGOING";

    public string ExportDir { get; set; } = "/INCOMING";
    
    public string ArchiveDir { get; set; } = "/ARCHIVE";
}