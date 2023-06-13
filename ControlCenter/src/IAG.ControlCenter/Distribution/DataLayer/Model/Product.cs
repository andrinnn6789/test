using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class Product : BaseEntityWithTenant
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductType ProductType { get; set; }
    public Product DependsOn { get; set; }
    public Guid? DependsOnProductId { get; set; }

    public ICollection<Release> Releases { set; get; }
    public ICollection<Installation> Installations { set; get; }
}