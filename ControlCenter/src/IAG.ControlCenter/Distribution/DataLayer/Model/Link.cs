using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class Link : BaseEntityWithTenant
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}