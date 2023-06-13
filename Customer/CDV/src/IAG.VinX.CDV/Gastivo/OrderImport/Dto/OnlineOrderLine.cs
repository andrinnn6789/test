using System;

namespace IAG.VinX.CDV.Gastivo.OrderImport.Dto;

[Serializable]
public class OnlineOrderLine
{
    public int Id { get; set; }
    public decimal OrderedBaseQuantity { get; set; }
    public decimal? UnitPrice { get; set; }
}