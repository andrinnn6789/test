using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using IAG.InstallClient.Models;

using Microsoft.AspNetCore.Authorization;

namespace IAG.InstallClient.Controllers;

public class ErrorController : Controller
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}