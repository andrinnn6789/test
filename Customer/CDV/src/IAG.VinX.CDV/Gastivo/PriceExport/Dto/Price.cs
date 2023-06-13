using System;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

namespace IAG.VinX.CDV.Gastivo.PriceExport.Dto;

[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord("\t")]
public class Price
{
    [FieldCaption("ARTICLE_NUMBER")]
    public string ArticleNumber { get; set; }
    
    [FieldCaption("CUSTOMER_NUMBER")]
    public string CustomerNumber { get; set; }
    
    [FieldCaption("PRICE")]
    public decimal ListPrice { get; set; }
    
    [FieldCaption("REGULAR_ORDER_LIST")]
    public int RegularOrderList { get; set; }
}