using System;

namespace IAG.VinX.IAG.ControlCenter.Distribution.DataLayer.Model;

public class IagCustomer
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int CustomerCategoryId { get; set; }
    public string Description { get; set; }
    public bool UsesVinX { get; set; }
    public bool UsesPerformX { get; set; }
}