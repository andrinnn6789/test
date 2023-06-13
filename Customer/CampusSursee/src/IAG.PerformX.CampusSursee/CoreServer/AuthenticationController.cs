using System.Threading.Tasks;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace IAG.PerformX.CampusSursee.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
public class AuthenticationController : BaseSybaseODataController
{
    private readonly IRealmHandler _realmHandler;

    public AuthenticationController(ISybaseConnectionFactory sybaseConnectionFactory, IRealmHandler realmHandler) : base(sybaseConnectionFactory)
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
}