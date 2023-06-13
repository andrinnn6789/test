using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class LinkListViewModel
{
    public List<LinkInfo> Links { get; set; }
    public string ErrorMessage { get; set; }
}