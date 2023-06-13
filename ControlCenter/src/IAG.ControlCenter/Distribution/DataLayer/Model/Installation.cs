using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class Installation : BaseEntityWithTenant
{
    public Guid CustomerId { get; set; }
    public Guid? ProductId { get; set; }
    public string ReleaseVersion { get; set; }
    public string Platform { get; set; }
    public string InstanceName { get; set; }
    public string Description { get; set; }

    public Product Product { get; set; }
}