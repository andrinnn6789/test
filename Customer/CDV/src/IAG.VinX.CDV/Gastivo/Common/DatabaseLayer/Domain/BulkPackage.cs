using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class BulkPackage
{
    public virtual int Id { get; set; }
    public virtual string SearchTerm { get; set; }
    public virtual string Abbreviation { get; set; }
    public virtual decimal QuantityPerBulkPackage { get; set; }
    public virtual short? BreakageAllowed { get; set; }
    public virtual short? IsPallet { get; set; }
    public virtual short? BulkPackagePerPallet { get; set; }
    public virtual string DescriptionForWeb { get; set; }
    public virtual string DescriptionForWebItalian { get; set; }
    public virtual string DescriptionForWebFrench { get; set; }
    public virtual string AbbreviationForWeb { get; set; }
    public virtual Article BulkPackageArticle { get; set; }
    public virtual Article BreakageArticle { get; set; }
    public virtual Article PalletArticle { get; set; }
}