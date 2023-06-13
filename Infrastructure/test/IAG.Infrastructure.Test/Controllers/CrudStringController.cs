using IAG.Infrastructure.Controllers;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Test.Controllers;

[Route("api/Test/[controller]")]
public class CrudStringController : NoDbCrudController<string, StringKey, CrudString>
{
    public CrudStringController()
    {
        Crud = new CrudString();
    }
}