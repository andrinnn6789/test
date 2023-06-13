using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class ProductCustomer : BaseEntityWithTenant
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }

    public Product Product { get; set; }
}