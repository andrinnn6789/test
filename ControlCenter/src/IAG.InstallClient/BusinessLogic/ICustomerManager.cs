using System;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.InstallClient.BusinessLogic;

public interface ICustomerManager
{
    Task<CustomerInfo> GetCustomerInformationAsync(Guid id);
    Task<CustomerInfo> GetCurrentCustomerInformationAsync();
    Task SetCurrentCustomerInformationAsync(CustomerInfo customerInfo);
}