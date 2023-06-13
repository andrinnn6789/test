using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.KWD.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.KWD.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataKwdController : BaseSybaseODataController
{
    public BaseDataKwdController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    /// Gets Articles, with customer specific property extensions
    /// </summary>
    /// <returns>A List of Articles</returns>
    [HttpGet("ArticleKwd")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleKwd>> GetArticlesKwd() => GetRequestedEntities<ArticleKwd>().ToList();

    /// <summary>
    /// Gets Addresses, with customer specific property extensions
    /// </summary>
    /// <returns>A List of Addresses</returns>
    [HttpGet("AddressKwd")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<AddressKwd>> GetAddressesKwd() => GetRequestedEntities<AddressKwd>().ToList();
}