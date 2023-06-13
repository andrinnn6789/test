using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.Swagger;
using IAG.PerformX.ibW.Authorization;
using IAG.PerformX.ibW.Dto.Web;
using IAG.PerformX.ibW.Sybase;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAG.PerformX.ibW.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpointWeb)]
[ClaimAuthorization(
    Scopes.RestScopeWeb, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class WebController : BaseSybaseODataController
{
    private readonly IRealmHandler _realmHandler;

    public WebController(ISybaseConnectionFactory sybaseConnectionFactory, IRealmHandler realmHandler) : base(sybaseConnectionFactory)
    {
        _realmHandler = realmHandler;
    }

    [AllowAnonymous]
    [HttpPost("RequestToken")]
    public async Task<ActionResult<RequestTokenResponse>> RequestToken(
        [FromBody] RequestTokenParameter parameter,
        [FromServices] IAttackDetection attackDetection,
        [FromServices] ITokenHandler tokenHandler)
    {
        return await _realmHandler.RequestToken(parameter, attackDetection, tokenHandler);
    }

    [HttpGet("Angebot")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Angebot>> GetAngebot()
    {
        return new WebClient(Connection).AddSubLinks(GetRequestedEntities<Angebot>());
    }

    [HttpGet("Lookup/ECommerce")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ECommerce>> GetECommerce()
    {
        return GetRequestedEntities<ECommerce>().ToList();
    }
}