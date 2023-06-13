using System;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class CustomerRegistration
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int CustomerCategoryId { get; set; }
    public string Description { get; set; }
}