using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.BusinessLogic.Model;

[ExcludeFromCodeCoverage]
public class InstalledRelease
{
    public string InstanceName { get; set; }
    public string ProductName { get; set; }
    public string Version { get; set; }
    public string CustomerPluginName { get; set ; }
}