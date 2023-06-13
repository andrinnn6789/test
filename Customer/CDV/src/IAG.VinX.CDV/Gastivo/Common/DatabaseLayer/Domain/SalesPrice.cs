using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class SalesPrice
{
    public virtual int Id { get; set; }
    public virtual Article Article { get; set; }
    public virtual PriceGroup PriceGroup { get; set; }
    public virtual decimal Price { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual DateTime? ValidFrom { get; set; }
    public virtual DateTime? ValidTo { get; set; }
}