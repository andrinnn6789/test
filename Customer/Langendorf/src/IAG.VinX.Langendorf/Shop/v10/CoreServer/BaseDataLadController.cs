using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Langendorf.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Langendorf.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataLadController : BaseSybaseODataController
{
    public BaseDataLadController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    /// get the articles, with customer extensions
    /// </summary>
    [HttpGet("ArticleLad")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleLad>> GetArticlesLad()
        => GetRequestedEntities<ArticleLad>().ToList();

}