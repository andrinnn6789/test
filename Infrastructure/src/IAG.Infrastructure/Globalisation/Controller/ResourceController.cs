using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Context;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IAG.Infrastructure.Globalisation.Controller;

[Route(InfrastructureEndpoints.Resource + "[controller]")]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.DefaultGroup)]
public class ResourceController : ODataCrudController<Guid, Model.Resource>
{

    public ResourceController(ResourceContext context) : base(context)
    {
    }
}