using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("Docs")]
public class DocsController : Controller
{
    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        return View("Index");
    }
}