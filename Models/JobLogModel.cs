using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class JobLogModel
{
    public string JobActionName { get; set; }
    public Guid JobId { get; set; }
    public string BackLinkAction { get; set; }
}