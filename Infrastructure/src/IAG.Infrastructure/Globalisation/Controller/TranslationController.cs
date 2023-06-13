using System;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Globalisation.Controller;

[Route(InfrastructureEndpoints.Resource + "[controller]")]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.DefaultGroup)]
public class TranslationController : ODataCrudController<Guid, Translation>
{
    public TranslationController(ResourceContext context) : base(context)
    {
    }
}