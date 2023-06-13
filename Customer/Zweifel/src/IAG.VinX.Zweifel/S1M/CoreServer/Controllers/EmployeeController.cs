using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Zweifel.S1M.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Zweifel.S1M.CoreServer.Controllers;

[Route("api/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class EmployeeController : BaseSybaseODataController
{
    public EmployeeController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    [HttpGet]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Employee>> GetEmployees()
    {
        return GetRequestedEntities<Employee>().ToList();
    }
}