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
using IAG.PerformX.ibW.Dto.Azure;
using IAG.PerformX.ibW.Sybase;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAG.PerformX.ibW.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpointAzure)]
[ClaimAuthorization(
    Scopes.RestScopeAzure, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class AzureController : BaseSybaseODataController
{
    private readonly IRealmHandler _realmHandler;

    public AzureController(ISybaseConnectionFactory sybaseConnectionFactory, IRealmHandler realmHandler) : base(sybaseConnectionFactory)
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

    [HttpGet("AdminUnit")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<AdminUnit>> GetAdminUnits()
    {
        return GetRequestedEntities<AdminUnit>().ToList();
    }

    [HttpGet("Person")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Person>> GetPerson()
    {
        return GetRequestedEntities<Person>().ToList();
    }

    [HttpPost("Person({id})/SetCloudLogin")]
    public ActionResult SetCloudLogin(int id, [FromBody] PersonChangeParam changeParam)
    {
        new AzureClient(Connection).SetCloudLogin(id, changeParam);
        return NoContent();
    }

    [HttpPost("Person({id})/ResetCloudLogin")]
    public ActionResult ResetCloudLogin(int id)
    {
        new AzureClient(Connection).ResetCloudLogin(id);
        return NoContent();
    }

    [HttpGet("Group")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Group>> GetGroups()
    {
        return GetRequestedEntities<Group>().ToList();
    }

    [HttpGet("Group({id})")]
    [ODataQueryEndpoint]
    public ActionResult<Group> GetGroup(int id)
    {
        return GetRequestedEntities<Group>().First(g => g.Id == id);
    }

    [HttpGet("GroupRelation")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<GroupRelation>> GetGroupRelations()
    {
        return GetRequestedEntities<GroupRelation>().ToList();
    }

    [HttpGet("GroupRelation({id})")]
    [ODataQueryEndpoint]
    public ActionResult<GroupRelation> GetGroupRelation(int id)
    {
        return GetRequestedEntities<GroupRelation>().First(g => g.Id == id);
    }

    [HttpGet("Owner")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Owner>> GetOwner()
    {
        return GetRequestedEntities<Owner>().ToList();
    }

    [HttpGet("Member")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Member>> GetMember()
    {
        return GetRequestedEntities<Member>().ToList();
    }
}