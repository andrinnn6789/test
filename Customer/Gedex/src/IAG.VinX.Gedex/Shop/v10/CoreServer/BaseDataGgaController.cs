using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Gedex.Shop.v10.DtoDirect;
using IAG.VinX.Shop.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.VinX.Gedex.Shop.v10.CoreServer;

[Route(ControllerEndpoints.ShopV10 + "[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiShopEndpointV10)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataGgaController : BaseSybaseODataController
{
    public BaseDataGgaController(ISybaseConnectionFactory sybaseConnectionFactory) : base(sybaseConnectionFactory)
    {
    }

    /// <summary>
    /// get the articles, with customer extensions
    /// </summary>
    [HttpGet("ArticleGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleGga>> GetArticlesGga()
        => GetRequestedEntities<ArticleGga>().ToList();

    /// <summary>
    /// get the wines, with customer extensions
    /// </summary>
    [HttpGet("WineGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<WineGga>> GetWinesGga()
        => GetRequestedEntities<WineGga>().ToList();

    /// <summary>
    /// get the amount of grape-types (AnzahlRebsorten), customer extension
    /// </summary>
    [HttpGet("GrapeAmountGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<GrapeAmountGga>> GetGrapeAmountsGga()
        => GetRequestedEntities<GrapeAmountGga>().ToList();

    /// <summary>
    /// get wine matches (PasstZu), customer extension
    /// </summary>
    [HttpGet("MatchGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<MatchGga>> GetMatchesGga()
        => GetRequestedEntities<MatchGga>().ToList();

    /// <summary>
    /// get the wine strengths (Schwere), customer extension
    /// </summary>
    [HttpGet("StrengthGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<StrengthGga>> GetStrengthsGga()
        => GetRequestedEntities<StrengthGga>().ToList();

    /// <summary>
    /// get wine tastes (Geschmack), customer extension
    /// </summary>
    [HttpGet("TasteGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<TasteGga>> GetTastesGga()
        => GetRequestedEntities<TasteGga>().ToList();

    /// <summary>
    /// get wine match relations (ArtikelPasstZu), customer extension
    /// </summary>
    [HttpGet("WineMatchRelationGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<WineMatchRelationGga>> GetWineMatchRelationsGga()
        => GetRequestedEntities<WineMatchRelationGga>().ToList();

    /// <summary>
    /// get wine taste relations (ArtikelGeschmack), customer extension
    /// </summary>
    [HttpGet("WineTasteRelationGga")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<WineTasteRelationGga>> GetWineTasteRelationsGga()
        => GetRequestedEntities<WineTasteRelationGga>().ToList();
}