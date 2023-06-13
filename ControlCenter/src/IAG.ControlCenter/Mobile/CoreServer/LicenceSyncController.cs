using System.Collections.Generic;

using IAG.ControlCenter.Mobile.BusinessLayer;
using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Context;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.ControlCenter.Mobile.CoreServer;

[Route(ControlCenterEndpoints.Mobile + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.ControlCenter)]
public class LicenceSyncController : ControllerBase
{
    private readonly LicenceAdmin _checker;

    public LicenceSyncController(MobileDbContext context, UserDatabaseAuthenticationPlugin userAuthPlugin)
    {
        _checker = new LicenceAdmin(context, userAuthPlugin);
    }

    /// <summary>
    /// Synchronizes mobile licences with remote systems.
    /// </summary>
    /// <remarks>
    /// This endpoint allows synchronization of mobile licences with remote systems.
    /// It updates the systems based on the provided licence sync request, removing any deleted entries.
    /// The method returns a list of changed mobile licences as a result of the synchronization.
    /// </remarks>
    /// <param name="request">The licence sync request containing the changes to be synchronized.</param>
    /// <returns>An action result containing a list of changed mobile licences.</returns>
    /// <response code="400">If the licence sync request is null.</response>
    /// <response code="200">Returns the list of changed mobile licences.</response>
    [HttpPost("Sync")]
    public ActionResult<List<MobileLicence>> Sync([FromBody] LicenceSync request)
    {
        if (request == null)
            return BadRequest();
        return _checker.SyncSystems(request);
    }


    /// <summary>
    /// Updates systems with the provided licence sync data.
    /// </summary>
    /// <remarks>
    /// This endpoint is used to update systems with the licence sync data provided in the request.
    /// It performs the necessary operations to add and update the systems based on the provided licence sync data.
    /// </remarks>
    /// <param name="request">The licence sync data containing the changes to be applied.</param>
    /// <returns>An IActionResult indicating the status of the update operation.</returns>
    /// <response code="400">If the licence sync data is null.</response>
    /// <response code="200">Indicates a successful update operation.</response>
    [HttpPost("Update")]
    public IActionResult Update([FromBody] LicenceSync request)
    {
        if (request == null)
            return BadRequest();
        _checker.UpdateSystems(request);
        return new OkResult();
    }

}