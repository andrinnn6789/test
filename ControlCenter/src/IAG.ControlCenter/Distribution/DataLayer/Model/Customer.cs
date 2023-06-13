using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class Customer : BaseEntityWithTenant
{
    public string Name { get; set; }
    public int CustomerCategoryId { get; set; }
    public string Description { get; set; }
    public ICollection<ProductCustomer> ProductCustomers { set; get; }
}