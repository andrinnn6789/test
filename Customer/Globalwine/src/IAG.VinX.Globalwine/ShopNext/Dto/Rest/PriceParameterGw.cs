using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// Parameter for batch calculation of prices. If CustomerId ist set, PriceGroupId is ignored.
/// </summary>
[DisplayName("PriceParameter")]
[DataContract]
public class PriceParameterGw
{
    [DataMember(Name="validDate")]
    public DateTime ValidDate { get; set; } = DateTime.Now;
    [DataMember(Name="customerId")]
    public int? AddressId { get; set; }
    [DataMember(Name="priceGroupId")]
    public int? PriceGroupId { get; set; }
    [DataMember(Name="division")]
    public int? Division { get; set; }

    [DataMember(Name = "articleParameters")]
    public List<ArticleParameterGw> ArticleParameters { get; set; } = new();
}

/// <summary>
/// List of article - quantity pairs to calculate the price
/// </summary>
[DisplayName("ArticleParameter")]
[DataContract]
public class ArticleParameterGw
{
    [DataMember(Name="articleId")]
    public int ArticleId { get; set; }
    [DataMember(Name="quantity")]
    public  decimal Quantity { get; set; }
}