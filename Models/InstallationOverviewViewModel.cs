using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class InstallationOverviewViewModel
{
    public List<InstallationViewModel> Installations { get; set; }
    public string ErrorMessage { get; set; }
}