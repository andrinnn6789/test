using System;
using System.Collections.Generic;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class CustomerInfo
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public int CustomerCategoryId { get; set; }
    public string Description { get; set; }
    public List<Guid> ProductIds { get; set; }
}