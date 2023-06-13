using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.InstallClient.Models.Mapper;

public class CustomerViewModelMapper : ObjectMapper<CustomerInfo, CustomerViewModel>
{
    protected override CustomerViewModel MapToDestination(CustomerInfo source, CustomerViewModel destination)
    {
        destination.CustomerId = source.Id;
        destination.CustomerName = source.CustomerName;
        destination.Description = source.Description;

        return destination;
    }
}