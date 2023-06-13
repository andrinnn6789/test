using System;
using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.InstallClient.BusinessLogic;

using Microsoft.AspNetCore.Mvc;
using IAG.InstallClient.Models;
using IAG.InstallClient.Models.Mapper;

using Microsoft.Extensions.Localization;

namespace IAG.InstallClient.Controllers;

public class CustomerController : BaseController
{
    public CustomerController(ICustomerManager customerManager, IStringLocalizer<CustomerController> stringLocalizer)
        :base(customerManager, stringLocalizer)
    {
    }

    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public async Task<IActionResult> Index()
    {
        return View(await GetCustomerViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Execute)]
    public async Task<IActionResult> Index(CustomerViewModel model)
    {
        if (!model.CustomerId.HasValue)
        {
            model.ErrorMessage = "Bitte geben Sie eine gültige Kunden-ID ein (Format 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx')";
                
            return View(model);
        }

        try
        {
            var customerInfo = await CustomerManager.GetCustomerInformationAsync(model.CustomerId.Value);
            await CustomerManager.SetCurrentCustomerInformationAsync(customerInfo);
            Customer = customerInfo;

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);

            return View(model);
        }
    }

    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public async Task<IActionResult> Edit()
    {
        var model = await GetCustomerViewModel();
        model.ForceEdit = true;
            
        return View("Index", model);
    }

    private async Task<CustomerViewModel> GetCustomerViewModel()
    {
        var mapper = new CustomerViewModelMapper();
        if (Customer != null)
        {
            return mapper.NewDestination(Customer);
        }

        try
        {
            var customerInfo = await CustomerManager.GetCurrentCustomerInformationAsync();
            if (customerInfo != null)
            {
                return mapper.NewDestination(customerInfo);
            }

            return new CustomerViewModel();
        }
        catch (Exception ex)
        {
            return new CustomerViewModel
            {
                ErrorMessage = MessageLocalizer.LocalizeException(ex)
            };
        }
    }
}