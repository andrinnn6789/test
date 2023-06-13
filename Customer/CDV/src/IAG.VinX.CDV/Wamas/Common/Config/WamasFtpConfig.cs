using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Wamas.Common.Config;

[ExcludeFromCodeCoverage]
public class WamasFtpConfig
{
    public string Url { get; set; } = "$$wamasFtpUrl$";

    public string User { get; set; } = "$$wamasFtpUser$";

    public string Password { get; set; } = "$$wamasFtpPassword$";

    public string ImportDir { get; set; } = "/toVinX";

    public string ExportDir { get; set; } = "/fromVinX";

    public string ImportSuccessDir { get; set; } = "/toVinX/success";

    public string ImportErrorDir { get; set; } = "/toVinX/error";
}