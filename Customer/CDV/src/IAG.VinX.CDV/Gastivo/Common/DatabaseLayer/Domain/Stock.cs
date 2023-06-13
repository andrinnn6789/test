using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

[ExcludeFromCodeCoverage]
public class Stock
{
    public virtual int Id { get; set; }
    public virtual Article Article { get; set; }
    public virtual Warehouse Warehouse { get; set; }
    public virtual decimal OnStock { get; set; }
    public virtual decimal Reserved { get; set; }
    public virtual decimal Provision { get; set; }
    public virtual decimal Ordered { get; set; }
    
    public virtual decimal MininumStockWebshop { get; set; }
}