using System;
using System.Collections.Generic;
using System.Linq;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;

public class CustomerInfoMapper : ObjectMapper<Customer, CustomerInfo>
{
    protected override CustomerInfo MapToDestination(Customer source, CustomerInfo destination)
    {
        destination.Id = source.Id;
        destination.CustomerName = source.Name;
        destination.CustomerCategoryId = source.CustomerCategoryId;
        destination.Description = source.Description;
        destination.ProductIds = source.ProductCustomers?.Select(x => x.ProductId).ToList() ?? new List<Guid>();

        return destination;
    }
}