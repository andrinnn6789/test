using System;
using System.Text.Json;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.InstallClient.BusinessLogic;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Localization;

namespace IAG.InstallClient.Controllers;

public abstract class BaseController : Controller
{
    private const string SessionKeyCustomerInfo = "Customer.Info";

    protected ICustomerManager CustomerManager { get; }
    protected IMessageLocalizer MessageLocalizer { get; }

    private CustomerInfo _customer;

    protected CustomerInfo Customer
    {
        get
        {
            if (_customer == null)
            {
                if (HttpContext.Session.TryGetValue(SessionKeyCustomerInfo, out var customerInfoBytes))
                {
                    _customer = customerInfoBytes.Length == 0 ? null : JsonSerializer.Deserialize<CustomerInfo>(customerInfoBytes);
                }
                else
                {
                    Customer = GetCustomer().Result;
                }
            }

            return _customer;
        }
        set
        {
            _customer = value;
            HttpContext.Session.Set(SessionKeyCustomerInfo, value == null ? new byte[0] : JsonSerializer.SerializeToUtf8Bytes(_customer));
        }
    }

    protected BaseController(ICustomerManager customerManager, IStringLocalizer stringLocalizer)
    {
        CustomerManager = customerManager;
        MessageLocalizer = new MessageLocalizer(stringLocalizer);
    }

    protected IActionResult ViewCustomerChecked(object model)
    {
        if (Customer == null)
        {
            return RedirectToAction("Index", "Customer");
        }

        return View(model);
    }

    private async Task<CustomerInfo> GetCustomer()
    {
        try
        {
            return await CustomerManager.GetCurrentCustomerInformationAsync();
        }
        catch (Exception)
        {
            return null;
        } 
    }
}