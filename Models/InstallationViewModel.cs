using System.Diagnostics.CodeAnalysis;

using IAG.InstallClient.BusinessLogic.Model;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class InstallationViewModel
{
    public string InstanceName { get; set; }
    public string ProductName { get; set; }
    public string Version { get; set; }
    public string CustomerPluginName { get; set ; }
    public string ServiceName { get; set; }
    public ServiceStatus? ServiceStatus { get; set; }
}