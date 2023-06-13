using IAG.Infrastructure.Controllers;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.IntegrationTest.Controller;

[Route("api/Test/[controller]")]
public class NumKeysODataController : ODataCrudController<int, NumKey>
{
    public NumKeysODataController(NumStringContext context) : base(context)
    {
    }
}