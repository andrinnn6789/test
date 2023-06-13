using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using IAG.InstallClient.Authorization;

namespace IAG.InstallClient.Controllers;

public class LoginController : BaseController
{
    private readonly ILoginManager _loginManager;

    public LoginController(ICustomerManager customerManager, ILoginManager loginManager, IStringLocalizer<HomeController> stringLocalizer) : base(customerManager, stringLocalizer)
    {
        _loginManager = loginManager;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> DoLogin(LoginViewModel model)
    {
        try
        {
            var token = await _loginManager.DoLoginAsync(model.Username, model.Password);
            BearerTokenCookieHandler.SetBearerToken(HttpContext, token);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            model.Password = null;
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
            return View("Index", model);
        }
    }

    public IActionResult Logout()
    {
        BearerTokenCookieHandler.ClearBearerToken(HttpContext);

        return RedirectToAction("Index");
    }
}