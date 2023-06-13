using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Mobile.BusinessLayer;
using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Context;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Admin;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.IdentityServer.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAG.ControlCenter.Mobile.CoreServer;

[Route(ControlCenterEndpoints.Mobile + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.ReaderScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.ControlCenter)]
public class LicenceAppController : ControllerBase
{
    private readonly LicenceCheck _checker;
    private readonly ResourceAdmin _admin;

    public LicenceAppController(MobileDbContext ccContext, ResourceContext resContext)
    {
        _checker = new LicenceCheck(ccContext);
        _admin = new ResourceAdmin(resContext);
    }

    /// <summary>
    /// Requests a token for authentication.
    /// </summary>
    /// <remarks>
    /// This endpoint allows anonymous access and is used to request a token for authentication.
    /// The token is generated based on the provided request token parameter.
    /// </remarks>
    /// <param name="requestTokenParameter">The request token parameter containing necessary information for token generation.</param>
    /// <param name="attackDetection">The attack detection service.</param>
    /// <param name="tokenHandler">The token handler service.</param>
    /// <param name="realmHandler">The realm handler service.</param>
    /// <returns>An action result containing the request token response.</returns>
    /// <response code="400">If the request token parameter is null.</response>
    /// <response code="200">If the request token is successfully generated.</response>
    [AllowAnonymous]
    [HttpPost("RequestToken")]
    public async Task<ActionResult<RequestTokenResponse>> RequestToken(
        [FromBody] SimpleRequestTokenParameter requestTokenParameter,
        [FromServices] IAttackDetection attackDetection,
        [FromServices] ITokenHandler tokenHandler,
        [FromServices] IRealmHandler realmHandler)
    {
        if (requestTokenParameter == null)
            return BadRequest();
        var requestTokenParam = requestTokenParameter.ToRequestTokenParameter()
            .ForRealm(UserDatabaseAuthenticationPlugin.RealmName);

        return await realmHandler.RequestToken(requestTokenParam, attackDetection, tokenHandler);
    }

    /// <summary>
    /// Checks the validity of a license.
    /// </summary>
    /// <remarks>
    /// This endpoint allows checking the validity of a license based on the provided license request.
    /// The license request contains the necessary information for license verification.
    /// </remarks>
    /// <param name="request">The license request containing the license information.</param>
    /// <returns>An action result containing the license response.</returns>
    /// <response code="400">If the license request is null.</response>
    /// <response code="200">If the license is successfully checked.</response>
    [HttpPost("Check")]
    public ActionResult<LicenceResponse> Check([FromBody] LicenceRequest request)
    {
        if (request == null)
            return BadRequest();
        return _checker.Check(request);
    }

    /// <summary>
    /// Frees a license.
    /// </summary>
    /// <remarks>
    /// This endpoint allows freeing a license based on the provided license request.
    /// The license request contains the necessary information for freeing the license.
    /// </remarks>
    /// <param name="request">The license request containing the license information.</param>
    /// <returns>An action result containing the license free response.</returns>
    /// <response code="400">If the license request is null.</response>
    /// <response code="200">If the license is successfully freed.</response>
    [HttpPost("Free")]
    public ActionResult<LicenceFreeResponse> Free([FromBody] LicenceRequest request)
    {
        if (request == null)
            return BadRequest();
        return _checker.Free(request);
    }

    /// <summary>
    /// Retrieves a flat list of translations.
    /// </summary>
    /// <remarks>
    /// This endpoint retrieves a flat list of translations based on the specified culture.
    /// The flat list contains translations with the specified culture and their corresponding values.
    /// </remarks>
    /// <param name="culture">The culture code for filtering translations.</param>
    /// <returns>An action result containing a flat list of translations.</returns>
    /// <response code="200">Returns the flat list of translations.</response>
    [HttpGet("GetFlat")]
    public ActionResult<IEnumerable<TranslationFlat>> GetFlat(string culture)
    {
        return _admin.GetFlat(culture);
    }

}