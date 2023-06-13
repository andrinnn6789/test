using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Schüwo.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Schüwo.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataSwController : BaseSybaseODataController
{
    public BaseDataSwController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    /// get the articles, with customer extensions
    /// </summary>
    [HttpGet("ArticleSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleSw>> GetArticlesSw()
        => GetRequestedEntities<ArticleSw>().ToList();

    /// <summary>
    /// get the article relations, customer extension
    /// </summary>
    [HttpGet("ArticleRelationSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleRelationSw>> GetArticelRelationSw()
        => GetRequestedEntities<ArticleRelationSw>().ToList();

    /// <summary>
    /// get the wine characters, customer extension
    /// </summary>
    [HttpGet("WineCharacterSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<WineCharacterSw>> GetCharacterSw()
        => GetRequestedEntities<WineCharacterSw>().ToList();

    /// <summary>
    /// get the customer marketing relations, customer extension
    /// </summary>
    [HttpGet("CustomerMarketingSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<CustomerMarketingSw>> GetCustomerMarketingSw()
        => GetRequestedEntities<CustomerMarketingSw>().ToList();

    /// <summary>
    /// get the containers, with customer extensions
    /// </summary>
    [HttpGet("TradingUnitSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<TradingUnitSw>> GetContainerSw()
        => GetRequestedEntities<TradingUnitSw>().ToList();

    /// <summary>
    /// get the marketing codes, customer extension
    /// </summary>
    [HttpGet("MarketingCodeSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<MarketingCodeSw>> GetMarketingCodeSw()
        => GetRequestedEntities<MarketingCodeSw>().ToList();

    /// <summary>
    /// get the selection codes, with customer extensions
    /// </summary>
    [HttpGet("SelectionCodeSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<SelectionCodeSw>> GetSelectionCodeSw()
        => GetRequestedEntities<SelectionCodeSw>().ToList();

    /// <summary>
    /// get the regions, with customer extensions
    /// </summary>
    [HttpGet("RegionSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<RegionSw>> GetRegionSw()
        => GetRequestedEntities<RegionSw>().ToList();

    /// <summary>
    /// get the countries, with customer extensions
    /// </summary>
    [HttpGet("CountrySw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<CountrySw>> GetCountrySw()
        => GetRequestedEntities<CountrySw>().ToList();

    /// <summary>
    /// get the e-commerce groups, with customer extensions
    /// </summary>
    [HttpGet("EcommerceGroupSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<EcommerceGroupSw>> GetEcommerceGroupSw()
        => GetRequestedEntities<EcommerceGroupSw>().ToList();

    /// <summary>
    /// get the addresses, with customer extensions
    /// </summary>
    [HttpGet("AddressSw")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<AddressSw>> GetAddressSw()
        => GetRequestedEntities<AddressSw>().ToList();
}