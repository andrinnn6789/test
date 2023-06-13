using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Mvc;


namespace IAG.ControlCenter.Distribution.CoreServer.Controller;

[Route(ControlCenterEndpoints.Distribution + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All)]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.ControlCenter)]
public class LinkAdminController : ControllerBase
{
    private readonly ILinkAdminHandler _handler;

    public LinkAdminController(ILinkAdminHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Synchronizes the links with the provided data.
    /// </summary>
    /// <remarks>
    /// This endpoint allows synchronizing the links by replacing the existing links with the provided data.
    /// The data should be provided in the request body as a list of LinkRegistration objects.
    /// </remarks>
    /// <param name="linkSyncRequest">The list of LinkRegistration objects containing the link data.</param>
    /// <returns>An action result containing the synchronized links.</returns>
    /// <response code="200">Indicates that the links have been successfully synchronized.</response>
    [HttpPost("Link/Sync")]
    public async Task<ActionResult<LinkInfo>> SyncLinks([FromBody] List<LinkRegistration> linkSyncRequest)
    {
        var links = await _handler.SyncLinksAsync(linkSyncRequest);

        return Ok(links);
    }   
}