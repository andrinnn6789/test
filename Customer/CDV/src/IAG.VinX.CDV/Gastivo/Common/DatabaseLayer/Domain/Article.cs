using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Article
{
    public virtual int Id { get; set; }
    public virtual ArticleCategory Category { get; set; }
    public virtual Filling Filling { get; set; }
    public virtual BulkPackage BulkPackage { get; set; }
    public virtual decimal? ArticleNumber { get; set; }
    public virtual string SearchTerm { get; set; }
    public virtual string Description { get; set; }
    [CanBeNull] public virtual string ProductTitle { get; set; }
    [CanBeNull] public virtual string ProductTitleItalian { get; set; }
    [CanBeNull] public virtual string ProductTitleFrench { get; set; }
    public virtual short ArticleType { get; set; }
    public virtual decimal? BasePrice { get; set; }
    public virtual DateTime ChangedOn { get; set; }
    public virtual decimal? EanCode1 { get; set; }
    public virtual decimal? EanCode2 { get; set; }
    public virtual decimal? EanCode3 { get; set; }
    public virtual decimal? EanCode4 { get; set; }
    public virtual Vat Vat { get; set; }
    public virtual Cycle Cycle { get; set; }
    [CanBeNull] public virtual Country Country { get; set; }
    [CanBeNull] public virtual ArticleECommerceGroup ECommerceGroup { get; set; }
    public virtual IList<SalesPrice> SalesPrices { get; set; }
    public virtual IList<StockMovement> StockMovements { get; set; }
    public virtual IList<Stock> Stocks { get; set; }
    public virtual int? DivisionId { get; set; }
    [CanBeNull] public virtual Region Region { get; set; }
    [CanBeNull] public virtual WineInfo WineInfo { get; set; }
}