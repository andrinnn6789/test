using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.Zweifel.S1M.Authorization;
using IAG.VinX.Zweifel.S1M.BusinessLogic;
using IAG.VinX.Zweifel.S1M.Dto.RequestModels;
using IAG.VinX.Zweifel.S1M.Dto.S1M;
using IAG.VinX.Zweifel.S1M.Sybase;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Zweifel.S1M.CoreServer.Controllers;

[Route("api/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class DeliveryController : BaseSybaseODataController
{
    private readonly IS1MDeliveryComposer _deliveryComposer;
    private readonly IDeliveryClient _deliveryClient;

    public DeliveryController(ISybaseConnectionFactory sybaseConnectionFactory, IS1MDeliveryComposer deliveryComposer, IDeliveryClient deliveryClient) : base(
        sybaseConnectionFactory)
    {
        _deliveryComposer = deliveryComposer;
        _deliveryComposer.SetConfig(Connection);
        _deliveryClient = deliveryClient;
        _deliveryClient.SetConfig(Connection);
    }

    [HttpGet]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<S1MExtDelivery>> GetDeliveries()
    {
        var deliveries = GetRequestedEntities<S1MDelivery>()
            .Where(x => x.Status == DeliveryStatus.Printed || x.Status == DeliveryStatus.Fixed).ToList();
        var mappedDeliveries = _deliveryComposer.ComposeDeliveries(deliveries);
        return Ok(mappedDeliveries);
    }

    [HttpPost("{id}/MarkAsDelivered")]
    public ActionResult MarkAsDelivered(int id, [FromBody] MarkDeliveredRequestModel reqModel)
    {
        var delivery = Connection.GetQueryable<S1MDelivery>().FirstOrDefault(d => d.DeliveryId == id);
        if (delivery == null) return NotFound($"Delivery with Id: {id} not found");
        try
        {
            Connection.BeginTransaction();
            _deliveryClient.MarkAsDelivered(id, reqModel);
            Connection.Commit();
        }
        catch (Exception e)
        {
            Connection.Rollback();
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }

        return Ok();
    }
}