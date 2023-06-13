using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class HomeViewModel
{
    public string CurrentReleaseVersion { get; set; }
    public Guid? SelfUpdateJobId { get; set; }
    public string ErrorMessage { get; set; }
}