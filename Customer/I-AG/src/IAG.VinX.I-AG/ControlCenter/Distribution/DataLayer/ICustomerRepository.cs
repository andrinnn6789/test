using System.Collections.Generic;

using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.VinX.IAG.ControlCenter.Distribution.DataLayer;

public interface ICustomerRepository
{
    IEnumerable<IagCustomer> GetCustomers();
}