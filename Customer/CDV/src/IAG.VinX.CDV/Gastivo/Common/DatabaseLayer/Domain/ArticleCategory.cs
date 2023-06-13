using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class ArticleCategory
{
    public virtual int Id { get; set; }
    public virtual string Description { get; set; }
    public virtual string DescriptionFrench { get; set; }
    public virtual string DescriptionItalian { get; set; }
    public virtual short? ArticleType { get; set; }

}