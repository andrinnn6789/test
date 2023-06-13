using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class OnlineOrderLine
{
    public virtual int Id { get; set; }
    public virtual OnlineOrder Order { get; set; }
    public virtual short? Position { get; set; }
    public virtual Article Article { get; set; }
    public virtual string Description { get; set; }
    public virtual decimal? OrderedQuantity { get; set; }
    public virtual decimal? UnitPrice { get; set; }
    public virtual decimal? Total { get; set; }
    public virtual decimal? Vat { get; set; }
}