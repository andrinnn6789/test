using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.Dto;

[Serializable]
[ExcludeFromCodeCoverage]
public class Article
{
    public string ArticleNumber { get; set; }
    public List<Translation> ArticleTextLong { get; set; }
    public string EanCode { get; set; }
    public string ArticleUnitCode { get; set; }
    public List<Translation> ArticleUnitText { get; set; }
    [CanBeNull] public List<Translation> WineType { get; set; }
    [CanBeNull] public List<Translation> WineRegion { get; set; }
    public decimal SellAmount { get; set; }
    public decimal VatRatePercent { get; set; }
    public decimal Price { get; set; }
    public ArticleType ArticleType { get; set; }
    public int DaysToDeliver { get; set; }
    [JsonConverter(typeof(StringEnumConverter))] public StockType StockType { get; set; }
    public int? ProductCategoryLvl1 { get; set; }
    public int? ProductCategoryLvl2 { get; set; }
    public string ImageUrl { get; set; }
    public int? RawMaterialOriginCode { get; set; }
    public List<Translation> ArticleDescriptionText { get; set; }
    public List<Translation> FitsWellWithDetail { get; set; }
    public List<Translation> Grapes { get; set; }
}