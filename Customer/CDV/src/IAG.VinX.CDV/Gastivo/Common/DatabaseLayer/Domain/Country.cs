using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Country
{
    public virtual int Id { get; set; }
    public virtual short IsoNumber { get; set; }
}