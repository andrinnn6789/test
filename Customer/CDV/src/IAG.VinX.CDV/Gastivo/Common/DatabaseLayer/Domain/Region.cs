using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Region
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string NameFrench { get; set; }
    public virtual string NameItalian { get; set; }
}