using System.Collections.Generic;
using System.Linq;

using IAG.Common.BaseData;
using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.PerformX.CampusSursee.Authorization;
using IAG.PerformX.CampusSursee.Dto.Address;

using Microsoft.AspNetCore.Mvc;


namespace IAG.PerformX.CampusSursee.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class AddressController : BaseSybaseODataController
{
    private readonly AddressClient _client;

    public AddressController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
        _client = new AddressClient(sybaseConnectionFactory.CreateConnection());
    }

    [HttpGet("Address")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Address>> GetAddresses()
    {
        return GetRequestedEntities<Address>().ToList();
    }

    [HttpPost("Address({id})/ChangeUserName")]
    public ActionResult ChangeUserName(int id, [FromBody] AddressChangeParam change)
    {
        _client.ChangeUsername(id, change.UserName);
        return NoContent();
    }

    [HttpGet("Document")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Document>> GetDocuments()
    {
        return GetRequestedEntities<Document>().ToList();
    }

    [HttpGet("Relation")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Relation>> GetRelations()
    {
        return GetRequestedEntities<Relation>().ToList();
    }

    [HttpGet("Registration")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Registration>> GetRegistrations()
    {
        return GetRequestedEntities<Registration>().ToList();
    }
}