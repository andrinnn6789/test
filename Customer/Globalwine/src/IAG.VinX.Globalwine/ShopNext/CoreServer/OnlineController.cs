using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.Authorization;
using IAG.VinX.Globalwine.ShopNext.DataAccess;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Mapper;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Globalwine.ShopNext.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class OnlineController : BaseSybaseODataController
{
    private readonly IBasketDataReader _basketDataReader;
    private readonly OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw> _client;

    public OnlineController(ISybaseConnectionFactory sybaseConnectionFactory, IBasketDataReader basketDataReader) : base(sybaseConnectionFactory)
    {
        _basketDataReader = basketDataReader;
        _client = new OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(Connection, _basketDataReader);
    }

    /// <summary>
    /// send a new basket
    /// </summary>
    /// <remarks>send a new basket. Control with 'action' whether to calculate only or to order. return is always the calculated basket with all relevant attributes.</remarks>
    [HttpPost("Basket")]
    public ActionResult<BasketRestGw> OnlineBasketPost([FromBody] BasketRestGw basket, 
        [FromServices] IBasketCalculator<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw> calculator)
    {
        return new BasketMapperToShop().NewDestination(
            calculator.Calculate(new BasketMapperFromShop().NewDestination(basket)));
    }

    /// <summary>
    /// get an online addresses
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("OnlineAddress({id:int})")]
    public ActionResult<OnlineAddressRestGw> GetOnlineaddressById(int id)
    {
        return new OnlineAddressMapperToShop().NewDestination(
            GetRequestedEntities<OnlineAddressGw>().First(c => c.Id == id));
    }

    /// <summary>
    /// send new online address
    /// </summary>
    [HttpPost("OnlineAddress")]
    public ActionResult<OnlineAddressRestGw> PostAddress([FromBody] OnlineAddressRestGw newAddress)
    {
        return new OnlineAddressMapperToShop().NewDestination(
            _client.NewOnlineAddress(new OnlineAddressMapperFromShop().NewDestination(newAddress)));
    }

    /// <summary>
    /// get the product details according to the query parameters
    /// </summary>
    [HttpPost("ProductDetail")]
    public ActionResult<IEnumerable<ProductDetailGw>> GetProductDetail([FromBody] PriceParameterGw priceParameter)
    {
        return new OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(Connection, _basketDataReader)
            .GetProductDetail(new PriceParameterMapperFromGw().NewDestination(priceParameter)).ToList();
    }
}