using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class CustomerSyncLogic
{
    private static readonly string ProductNameVinX = "VinX";
    private static readonly string ProductNamePerformX = "PerformX";
    private static readonly string ProductNameInstallerTool = "Installer";

    private readonly IMessageLogger _messageLogger;
    private readonly SyncResult _syncResult;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerAdminClient _customerAdminClient;
    private readonly IProductAdminClient _productAdminClient;


    public CustomerSyncLogic(ICustomerAdminClient customerAdminClient, IProductAdminClient productAdminClient,
        ICustomerRepository customerRepository, IMessageLogger messageLogger, SyncResult syncResult)
    {
        _messageLogger = messageLogger;
        _syncResult = syncResult;
        _customerRepository = customerRepository;
        _customerAdminClient = customerAdminClient;
        _productAdminClient = productAdminClient;
    }

    public async Task DoSyncAsync(IJobHeartbeatObserver jobHeartbeatObserver)
    {
        var productVinX = await _productAdminClient.RegisterProductAsync(ProductNameVinX, ProductType.IagService);
        var productPerformX = await _productAdminClient.RegisterProductAsync(ProductNamePerformX, ProductType.IagService);
        var installerTool = await _productAdminClient.RegisterProductAsync(ProductNameInstallerTool, ProductType.Updater);
        var registeredCustomers = await _customerAdminClient.GetCustomersAsync();
        var customers = _customerRepository.GetCustomers();

        var registeredCustomerIds = registeredCustomers.Select(c => c.Id).ToHashSet();
        foreach (var customer in customers.Where(c => !registeredCustomerIds.Contains(c.CustomerId)))
        {
            await AddCustomerWithProducts(customer, GetCustomersProductIds(customer, productVinX.Id, productPerformX.Id, installerTool.Id));
            jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
        }
    }

    private async Task AddCustomerWithProducts(IagCustomer customer, IEnumerable<Guid> productIds)
    {
        try
        {
            var customerId = (await _customerAdminClient.RegisterCustomerAsync(customer.CustomerId, customer.CustomerName, customer.CustomerCategoryId, customer.Description)).Id;
            await _customerAdminClient.AddProductsAsync(customerId, productIds);
            _syncResult.SuccessCount++;
        }
        catch (Exception ex)
        {
            _messageLogger.AddMessage(MessageTypeEnum.Debug, ResourceIds.AddCustomerInfo, customer.CustomerName, string.Join(", ", GetCustomersProductNames(customer)));
            _messageLogger.LogException(ex);
            _syncResult.ErrorCount++;
        }
    }

    private static IEnumerable<Guid> GetCustomersProductIds(IagCustomer customer, Guid productIdVinX, Guid productIdPerformX, Guid productIdInstallerTool)
    {
        if (customer.UsesVinX) yield return productIdVinX;
        if (customer.UsesPerformX) yield return productIdPerformX;
        yield return productIdInstallerTool;
    }

    private static IEnumerable<string> GetCustomersProductNames(IagCustomer customer)
    {
        if (customer.UsesVinX) yield return ProductNameVinX;
        if (customer.UsesPerformX) yield return ProductNamePerformX;
        yield return ProductNameInstallerTool;
    }
}