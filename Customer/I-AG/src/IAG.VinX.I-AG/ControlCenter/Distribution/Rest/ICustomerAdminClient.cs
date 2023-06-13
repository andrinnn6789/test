using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public interface ICustomerAdminClient
{
    Task<CustomerInfo> RegisterCustomerAsync(Guid customerId, string customerName, int categoryId, string description);
    Task<List<CustomerInfo>> GetCustomersAsync();
    Task AddProductsAsync(Guid customerId, IEnumerable<Guid> productIds);
}