using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Boucherville.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Boucherville.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataBovController : BaseSybaseODataController
{
    public BaseDataBovController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    /// get the addresses, with customer extensions
    /// </summary>
    [HttpGet("AddressBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<AddressBov>> GetAddressesBov()
        => GetRequestedEntities<AddressBov>().ToList();

    /// <summary>
    /// get the articles, with customer extensions
    /// </summary>
    [HttpGet("ArticleBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleBov>> GetArticlesBov()
        => GetRequestedEntities<ArticleBov>().ToList();

    /// <summary>
    /// get the countries, with customer extensions
    /// </summary>
    [HttpGet("CountryBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<CountryBov>> GetCountriesBov()
        => GetRequestedEntities<CountryBov>().ToList();

    /// <summary>
    /// get the delivery conditions, with customer extensions
    /// </summary>
    [HttpGet("DeliveryConditionBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<DeliveryConditionBov>> GetDeliveryConditionsBov()
        => GetRequestedEntities<DeliveryConditionBov>().ToList();

    /// <summary>
    /// get the e-commerce groups of the articles, with customer extensions
    /// </summary>
    [HttpGet("ECommerceGroupBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ECommerceGroupBov>> GetECommerceGroupsBov()
        => GetRequestedEntities<ECommerceGroupBov>().ToList();

    /// <summary>
    /// get the fillings of the articles, with customer extensions
    /// </summary>
    [HttpGet("FillingBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<FillingBov>> GetFillingsBov()
        => GetRequestedEntities<FillingBov>().ToList();

    /// <summary>
    /// get the grapes, with customer extensions
    /// </summary>
    [HttpGet("GrapeBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<GrapeBov>> GetGrapesBov()
        => GetRequestedEntities<GrapeBov>().ToList();

    /// <summary>
    /// get the payment conditions, with customer extensions
    /// </summary>
    [HttpGet("PaymentConditionBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<PaymentConditionBov>> GetPaymentConditionsBov()
        => GetRequestedEntities<PaymentConditionBov>().ToList();

    /// <summary>
    /// get the predicates, customer extension
    /// </summary>
    [HttpGet("PredicateBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<PredicateBov>> GetPredicatesBov()
        => GetRequestedEntities<PredicateBov>().ToList();

    /// <summary>
    /// get the producers, with customer extensions
    /// </summary>
    [HttpGet("ProducerBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ProducerBov>> GetProducersBov()
        => GetRequestedEntities<ProducerBov>().ToList();

    /// <summary>
    /// get the receipt types, with customer extensions
    /// </summary>
    [HttpGet("ReceiptTypeBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ReceiptTypeBov>> GetReceiptTypesBov()
        => GetRequestedEntities<ReceiptTypeBov>().ToList();

    /// <summary>
    /// get the regions, with customer extensions
    /// </summary>
    [HttpGet("RegionBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<RegionBov>> GetRegionsBov()
        => GetRequestedEntities<RegionBov>().ToList();

    /// <summary>
    /// get the selection codes, with customer extensions
    /// </summary>
    [HttpGet("SelectionCodeBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<SelectionCodeBov>> GetSelectionCodesBov()
        => GetRequestedEntities<SelectionCodeBov>().ToList();

    /// <summary>
    /// get the trading units, with customer extensions
    /// </summary>
    [HttpGet("TradingUnitBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<TradingUnitBov>> GetTradingUnitsBov()
        => GetRequestedEntities<TradingUnitBov>().ToList();

    /// <summary>
    /// get the wines, with customer extensions
    /// </summary>
    [HttpGet("WineBov")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<WineBov>> GetWinesBov()
        => GetRequestedEntities<WineBov>().ToList();
}