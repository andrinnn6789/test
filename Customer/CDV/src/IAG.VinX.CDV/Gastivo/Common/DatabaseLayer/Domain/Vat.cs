using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Vat
{
    public virtual int Id { get; set; }
    public virtual string Description { get; set; }
    public virtual decimal Percentage { get; set; }
}