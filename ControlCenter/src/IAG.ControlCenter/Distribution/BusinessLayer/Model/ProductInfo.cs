using System;
using IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class ProductInfo
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public ProductType ProductType { get; set; }
    public Guid? DependsOnProductId { get; set; }
    public string Description { get; set; }
}