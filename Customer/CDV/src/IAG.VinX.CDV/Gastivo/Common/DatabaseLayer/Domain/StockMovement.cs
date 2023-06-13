using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class StockMovement
{
    public virtual int Id { get; set; }
    public virtual decimal Quantity { get; set; }
    public virtual decimal BulkPackageQuantity { get; set; }
    public virtual Article Article { get; set; }
    public virtual Warehouse Warehouse { get; set; }
    public virtual Address Address { get; set; }
    public virtual DateTime Date { get; set; }
    public virtual short MovementType { get; set; }
}