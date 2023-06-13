using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.PerformX.CampusSursee.Authorization;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Registration;
using IAG.PerformX.CampusSursee.Sybase;

using Microsoft.AspNetCore.Mvc;


namespace IAG.PerformX.CampusSursee.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class RegistrationController : BaseSybaseODataController
{
    private readonly RegistrationClient _client;

    public RegistrationController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
        _client = new RegistrationClient(sybaseConnectionFactory.CreateConnection());
    }

    [HttpGet("Address({id})")]
    [ODataQueryEndpoint]
    public ActionResult<RegistrationAddress> GetAddress(int id)
    {
        var item = GetRequestedEntities<RegistrationAddress>().Where(a => a.Id == id).ToList().FirstOrDefault();
        if (item == null)
            return NotFound();
        return item;
    }

    [HttpGet("Address")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<RegistrationAddress>> GetAddress()
    {
        return GetRequestedEntities<RegistrationAddress>().ToList();
    }

    [HttpPost("AddressChange")]
    public ActionResult<RegistrationAddress> PostAddressChange([FromBody] RegistrationAddress change)
    {
        if (change == null)
            return BadRequest();
        return _client.AddressChange(change);
    }

    [HttpPost("AddressNew")]
    public ActionResult<RegistrationAddress> PostAddressNew([FromBody] RegistrationAddress newAddress)
    {
        if (newAddress == null)
            return BadRequest();
        return _client.AddressNew(newAddress);
    }

    [HttpPost("Registration")]
    public ActionResult<RegistrationPending> PostRegistration([FromBody] RegistrationPending registration)
    {
        if (registration == null)
            return BadRequest();
        return _client.RegistrationNew(registration);
    }

    [HttpPost("RegistrationWithAddress")]
    public ActionResult<RegistrationPending> PostRegistrationWithAddress([FromBody] RegistrationWithAddress registrationWithAddress)
    {
        if (registrationWithAddress == null)
            return BadRequest();
        return _client.RegistrationWithAddress(registrationWithAddress);
    }

    [HttpGet("Registration({id})")]
    [ODataQueryEndpoint]
    public ActionResult<RegistrationPending> GetRegistration(int id)
    {
        var item = GetRequestedEntities<RegistrationPending>().Where(a => a.Id == id).ToList().FirstOrDefault();
        if (item == null)
            return NotFound();
        return item;
    }

    [HttpPost("Registration({id})/UpdateWeblink")]  
    public ActionResult ChangeWeblink(int id, [FromBody] UpdateWeblinkParam changeObject)
    {
        _client.UpdateWeblink(id, changeObject.Weblink);
        return NoContent();
    }
}