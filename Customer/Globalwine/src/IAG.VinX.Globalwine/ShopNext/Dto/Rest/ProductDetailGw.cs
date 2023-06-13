using System.ComponentModel;
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// detail of a product in the context of a customer and / or price group
/// </summary>
[DataContract]
[DisplayName("ProductDetail")]
public class ProductDetailGw
{ 
    /// <summary>
    /// id of the article
    /// </summary>
    [DataMember(Name="articleId")]
    public int ArticleId { get; set; }

    /// <summary>
    /// id of the price group
    /// </summary>
    [DataMember(Name="priceGroupId")]
    public int? PriceGroupId { get; set; }

    /// <summary>
    /// available stock in base units of the article
    /// </summary>
    [DataMember(Name="stock")]
    public decimal Stock { get; set; }

    /// <summary>
    /// quantity in base units of the article
    /// </summary>
    [DataMember(Name="quantity")]
    public decimal Quantity { get; set; }

    /// <summary>
    /// price excl VAT for the article in the context of the query parameters, but not promotion
    /// </summary>
    [DataMember(Name="price")]
    public decimal Price { get; set; }

    /// <summary>
    /// price excl VAT for the article in the context of the query parameters for promotions
    /// </summary>
    [DataMember(Name="pricePromotion")]
    public decimal? PricePromotion { get; set; }

    /// <summary>
    /// applicable tax rate in percent
    /// </summary>
    [DataMember(Name="applicableTaxRate")]
    public decimal ApplicableTaxRate { get; set; }

    /// <summary>
    /// sold out / Art_Ausverkauft
    /// </summary>
    [DataMember(Name="temporairySoldOut")]
    public bool TemporairySoldOut { get; set; }
}