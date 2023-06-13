using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class NewInstallationViewModel : ChangeInstallationViewModel
{
    private static readonly string[] DefaultInstances = {"Test", "Prod"};

    public IEnumerable<string> SuggestedInstances => DefaultInstances.Concat(ExistingInstances ?? Enumerable.Empty<string>()).Distinct();
    public IEnumerable<string> ExistingInstances { get; set; }
}