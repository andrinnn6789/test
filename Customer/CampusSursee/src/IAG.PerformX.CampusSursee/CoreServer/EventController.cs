using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.PerformX.CampusSursee.Authorization;
using IAG.PerformX.CampusSursee.Dto.Event;

using Microsoft.AspNetCore.Mvc;


namespace IAG.PerformX.CampusSursee.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class EventController : BaseSybaseODataController
{
    public EventController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    [HttpGet("Event")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Event>> GetEvents()
    {
        return GetRequestedEntities<Event>().ToList();
    }

    [HttpGet("Event({id})")]
    [ODataQueryEndpoint]
    public ActionResult<Event> GetEvent(int id)
    {
        var events = GetRequestedEntities<Event>().Where(e => e.Id == id).ToList();
        return events.Count switch
        {
            0 => NotFound(),
            1 => events[0],
            _ => Conflict(),
        };
    }

    [HttpGet("Occurence")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Occurence>> GetOccurences()
    {
        return GetRequestedEntities<Occurence>().ToList();
    }

    [HttpGet("Additional")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Additional>> GetAdditionals()
    {
        return GetRequestedEntities<Additional>().ToList();
    }

    [HttpGet("EventModule")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<EventModule>> GetEventModule()
    {
        return GetRequestedEntities<EventModule>().ToList();
    }
}