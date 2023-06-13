using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class UpdateInstallationViewModel : ChangeInstallationViewModel
{
    public string ServiceName { get; set; }
}