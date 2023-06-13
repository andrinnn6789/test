using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Recommendation
{
    public virtual int Id { get; set; }
    public virtual string Description { get; set; }
    public virtual string DescriptionFrench { get; set; }
    public virtual string DescriptionItalian { get; set; }
}