using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Filling
{
    public virtual int Id { get; set; }
    public virtual Article Article { get; set; }
    public virtual decimal? ContentInCl { get; set; }
    public virtual string SearchTerm { get; set; }
    public virtual string Abbreviation { get; set; }
    public virtual string AbbreviationForWeb { get; set; }
    public virtual string DescriptionForWeb { get; set; }
}