using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public class CustomerAdminClient : RestClient, ICustomerAdminClient
{
    private const string CustomerAdminEndpoint = "CustomerAdmin/Customer";

    public CustomerAdminClient(IHttpConfig config, IRequestResponseLogger logger = null) : base(config, logger)
    {
    }

    public async Task<CustomerInfo> RegisterCustomerAsync(Guid customerId, string customerName, int categoryId, string description)
    {
        try
        {
            var requestBody = new CustomerRegistration()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerCategoryId = categoryId,
                Description = description
            };
            var request = new JsonRestRequest(HttpMethod.Post, CustomerAdminEndpoint);
            request.SetJsonBody(requestBody);

            return await PostAsync<CustomerInfo>(request);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.RegisterCustomerError, ex, customerName, categoryId);
        }
    }

    public async Task<List<CustomerInfo>> GetCustomersAsync()
    {
        try
        {
            return await GetAsync<List<CustomerInfo>>(CustomerAdminEndpoint);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.GetCustomersError, ex);
        }
    }

    public async Task AddProductsAsync(Guid customerId, IEnumerable<Guid> productIds)
    {
        try
        {
            var request = new JsonRestRequest(HttpMethod.Post, CustomerAdminEndpoint + "/{customerId}/AddProducts");
            request.SetUrlSegment("customerId", customerId);
            request.SetJsonBody(productIds);

            var response = await ExecuteAsync(request);
            await response.CheckResponse();
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.AddProductsForCustomerError, ex, customerId);
        }
    }
}