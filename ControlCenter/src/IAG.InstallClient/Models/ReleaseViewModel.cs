using System;
using System.Diagnostics.CodeAnalysis;

using IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.InstallClient.Models;

[ExcludeFromCodeCoverage]
public class ReleaseViewModel
{
    public Guid ProductId { get; set; }
    public Guid ReleaseId { get; set; }
    public string ProductName { get; set; }
    public string Plattform { get; set; }
    public string ReleaseVersion { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ProductType ProductType { get; set ; }
    public Guid? DependsOnProductId { get; set; }
}