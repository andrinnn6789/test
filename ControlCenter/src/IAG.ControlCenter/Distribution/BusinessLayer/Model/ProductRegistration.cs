
using System;

using IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class ProductRegistration
{
    public string ProductName { get; set; }
    public ProductType Type { get; set; }
    public Guid? DependsOnProductId { get; set; }
    public string Description { get; set; }
}