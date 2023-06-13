using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.Controller;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.Authorization;
using IAG.VinX.Globalwine.ShopNext.BusinessLogic;
using IAG.VinX.Globalwine.ShopNext.DataAccess;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Microsoft.AspNetCore.Mvc;

using DeliveryConditionGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.DeliveryConditionGw;
using ProducerGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.ProducerGw;

namespace IAG.VinX.Globalwine.ShopNext.CoreServer;

[Route("api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/[controller]")]
[ApiExplorerSettings(GroupName = SwaggerEndpointProvider.ApiEndpoint)]
[ClaimAuthorization(
    Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All,
    ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All
)]
public class BaseDataController : BaseSybaseODataController
{
    private readonly OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw> _client;

    public BaseDataController(ISybaseConnectionFactory sybaseConnectionFactory, IBasketDataReader basketDataReader) : base(sybaseConnectionFactory)
    {
        _client = new OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(Connection, basketDataReader);
    }

    /// <summary>
    /// get the articles
    /// </summary>
    /// <remarks>Get the articles. Allowed filters are all attributes of the root object.</remarks>
    [HttpGet("Article")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ArticleGw>> GetArticles([FromServices] IStockCalculator stockCalculator)
    {
        var articles = GetRequestedEntities<ArticleGw>().ToList();
        // Option: use IncludeChildren when ready (table-alias "Article" is not resolved)
        // var articles = GetRequestedEntities<Article>()
        //     .LoadChildren(a => a.Compositions)
        //     .LoadChildren(a => a.Prices)
        //     .LoadChildren(a => a.Ratings)
        //     .LoadChildren(a => a.Recommendations);
        // return articles.ToList();

        var compositions = Connection.GetQueryable<CompositionGw>().OrderBy(c => c.ArticleId).ToList();
        var prices = Connection.GetQueryable<PriceGw>().OrderBy(c => c.ArticleId).ToList();
        var ratings = Connection.GetQueryable<RatingGw>().OrderBy(c => c.ArticleId).ToList();
        var recommendations = Connection.GetQueryable<RecommendationGw>().OrderBy(c => c.ArticleId).ToList();
        var stockData = stockCalculator.GetStock(new List<ArticleParameter>()).OrderBy(c => c.ArticleId).ToList();
        foreach (var article in articles)
        {
            article.Compositions = compositions.Where(c => c.ArticleId == article.Id).ToList();
            article.Prices = prices.Where(c => c.ArticleId == article.Id).ToList();
            article.Ratings = ratings.Where(c => c.ArticleId == article.Id).ToList();
            article.Recommendations = recommendations.Where(c => c.ArticleId == article.Id).ToList();
            article.QtyStock = stockData.Where(s => s.ArticleId == article.Id).Sum(a => a.QuantityAvailable);
        }
        return articles;
    }

    /// <summary>
    /// get the contacts with address data
    /// </summary>
    [HttpGet("ContactWithAddress")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ContactWithAddress>> GetContactsWithAddress()
    {
        var contactsWithAddresses = GetRequestedEntities<ContactWithAddressFlat>();
        return new AddressTransformer().Transform(contactsWithAddresses);
    }

    /// <summary>
    /// get the contacts
    /// </summary>
    [HttpGet("Contact")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ContactGw>> GetContacts()
    {
        return GetRequestedEntities<ContactGw>().ToList();
    }

    /// <summary>
    /// get a contact
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("Contact({id})")]
    public ActionResult<ContactGw> GetContactById(int id)
    {
        return GetRequestedEntities<ContactGw>().First(c => c.Id == id);
    }

    /// <summary>
    /// update an existing contact
    /// </summary>
    /// <param name="id"></param>
    /// <param name="contact"></param>
    [HttpPut("Contact({id})")]
    public ActionResult<ContactGw> UpdateContactId(int id, [FromBody]ContactGw contact)
    {
        contact.Id = id;
        return _client.UpdateContact(contact);
    }

    /// <summary>
    /// send a new contact
    /// </summary>
    /// <param name="contact"></param>
    [HttpPost("Contact")]
    public ActionResult<ContactGw> PostContact([FromBody]ContactGw contact)
    {
        return _client.NewContact(contact);
    }

    /// <summary>
    /// get the payment conditions
    /// </summary>
    [HttpGet("PaymentCondition")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<PaymentConditionGw>> GetPaymentConditions()
    { 
        return GetRequestedEntities<PaymentConditionGw>().ToList();
    }

    /// <summary>
    /// get the payment conditions
    /// </summary>
    [HttpGet("DeliveryCondition")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<DeliveryConditionGw>> GetDeliveryConditions()
    { 
        return GetRequestedEntities<DeliveryConditionGw>().ToList();
    }

    /// <summary>
    /// get the carriers
    /// </summary>
    [HttpGet("Carrier")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<CarrierGw>> GetCarriers()
    { 
        return GetRequestedEntities<CarrierGw>().ToList();
    }

    /// <summary>
    /// get the salutations
    /// </summary>
    [HttpGet("Salutation")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<SalutationGw>> GetSalutation()
    { 
        return GetRequestedEntities<SalutationGw>().ToList();
    }

    /// <summary>
    /// get the producers
    /// </summary>
    [HttpGet("Producer")]
    [ODataQueryEndpoint]
    public ActionResult<IEnumerable<ProducerGw>> GetProducers()
    { 
        return GetRequestedEntities<ProducerGw>().ToList();
    }
}