using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.VinX.Zweifel.S1M.Authorization;
using IAG.VinX.Zweifel.S1M.BusinessLogic;
using IAG.VinX.Zweifel.S1M.Dto.RequestModels;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Zweifel.S1M.CoreServer.Controllers;

[Route("api/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class MediaController : BaseSybaseODataController
{
    private readonly IS1MMediaWriter _s1MMediaWriter;

    public MediaController(ISybaseConnectionFactory sybaseConnectionFactory, IS1MMediaWriter s1MMediaWriter) : base(sybaseConnectionFactory)
    {
        _s1MMediaWriter = s1MMediaWriter;
    }

    [HttpPost("UploadMedia")]
    public ActionResult UploadMedia([FromBody] UploadMediaRequestModel reqModel)
    {
        var result = _s1MMediaWriter.WriteMedia(reqModel);
        if (!result)
            return UnprocessableEntity($"Failed to write Media for Document with Number {reqModel.DocumentNumber}");
        return Ok();
    }
}