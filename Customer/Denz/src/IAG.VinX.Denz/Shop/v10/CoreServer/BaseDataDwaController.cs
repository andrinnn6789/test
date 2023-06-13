using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Denz.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Denz.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataDwaController : BaseSybaseODataController
{
    public BaseDataDwaController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    /// get the articles, with customer extensions
    /// </summary>
    [HttpGet("ArticleDwa")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleDwa>> GetArticlesDwa()
        => GetRequestedEntities<ArticleDwa>().ToList();

    /// <summary>
    /// get the wine infos, with customer extensions
    /// </summary>
    [HttpGet("WineDwa")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<WineDwa>> GetWineInfosDwa()
        => GetRequestedEntities<WineDwa>().ToList();
}