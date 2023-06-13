using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Cycle
{
    public virtual int Id { get; set; }
    public virtual string Description { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual short? PriceList { get; set; }
}