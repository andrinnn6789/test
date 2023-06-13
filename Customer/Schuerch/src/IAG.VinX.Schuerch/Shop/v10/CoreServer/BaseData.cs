using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Schuerch.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Schuerch.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "Schuerch/" + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseData : BaseSybaseODataController
{
    public BaseData(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    ///     get the fillings, with customer extensions
    /// </summary>
    [HttpGet("Filling")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<FillingSchuerch>> GetFillingsSchuerch()
    {
        return GetRequestedEntities<FillingSchuerch>().ToList();
    }

    /// <summary>
    ///     get the articles, with customer extensions
    /// </summary>
    [HttpGet("Article")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleSchuerch>> GetArticlesSchuerch()
    {
        return GetRequestedEntities<ArticleSchuerch>().ToList();
    }
}