using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public interface ICustomerAdminHandler
{
    Task<CustomerInfo> RegisterCustomerAsync(CustomerRegistration customer);
    Task<IEnumerable<CustomerInfo>> GetCustomersAsync();
    Task AddProductsAsync(Guid customerId, IEnumerable<Guid> productsToAdd);
    Task RemoveProductsAsync(Guid customerId, IEnumerable<Guid> productsToRemove);
}