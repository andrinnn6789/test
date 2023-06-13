using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Startup.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.IntegrationTest.Controller;

[Route("api/Test/[controller]")]
public class NumKeysContextController : ContextCrudController<int, NumKey>
{
    public NumKeysContextController(NumStringContext context)
    {
        Context = context;
    }

    [HttpGet("AllFiltered")]
    public ActionResult<IEnumerable<NumKey>> GetAllFiltered()
    {
        HttpContext.Items.Add(JsonContractResolver.MemberFilterKey, new List<MemberInfo>
        {
            typeof(NumKey).GetMembers().First()
        });
        return Ok(Context.Set<NumKey>().AsNoTracking().ToList());
    }
}