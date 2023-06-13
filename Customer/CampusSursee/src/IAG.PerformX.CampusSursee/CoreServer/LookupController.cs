using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;

using IAG.Common.ArchiveProvider;
using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.PerformX.CampusSursee.Authorization;
using IAG.PerformX.CampusSursee.Dto.Lookup;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using FileHandler = IAG.PerformX.CampusSursee.Sybase.FileHandler;


namespace IAG.PerformX.CampusSursee.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class LookupController : BaseSybaseODataController
{
    private readonly ILogger<LookupController> _logger;

    public LookupController(ISybaseConnectionFactory sybaseConnectionFactory, ILogger<LookupController> logger) : base(sybaseConnectionFactory)
    {
        _logger = logger;
    }

    [HttpGet("Salutation")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Salutation>> GetSalutations()
    {
        return GetRequestedEntities<Salutation>().ToList();
    }

    [HttpGet("RegistrationStatus")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<RegistrationStatus>> GetRegistrationStatus()
    {
        return GetRequestedEntities<RegistrationStatus>().ToList();
    }

    [HttpGet("EventStatus")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<EventStatus>> GetEventStatusStatus()
    {
        return GetRequestedEntities<EventStatus>().ToList();
    }

    [HttpGet("EventKind")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<EventKind>> GetEventKinds()
    {
        return GetRequestedEntities<EventKind>().ToList();
    }

    [HttpGet("AddressRelationKind")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<AddressRelationKind>> GetAddressRelationKinds()
    {
        return GetRequestedEntities<AddressRelationKind>().ToList();
    }

    [HttpGet("CommunicationChannel")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<CommunicationChannel>> GetCommunicationChannels()
    {
        return GetRequestedEntities<CommunicationChannel>().ToList();
    }

    [HttpGet("Country")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<Country>> GetCountries()
    {
        return GetRequestedEntities<Country>().ToList();
    }

    [HttpGet("Document({id})")]
    public IActionResult GetDocument(int id, [FromServices]IArchiveProviderFactory archiveProviderFactory)
    {
        try
        {
            var fileHandler = new FileHandler(Connection, archiveProviderFactory, _logger);
            var fileData = fileHandler.GetDocumentData(id);
            return File(fileData.docData, MediaTypeNames.Application.Pdf, fileData.docName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}