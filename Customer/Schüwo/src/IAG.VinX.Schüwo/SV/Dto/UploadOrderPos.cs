using IAG.VinX.Schüwo.SV.Dto.Interface;

using JetBrains.Annotations;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable IDE1006 // Naming Styles
namespace IAG.VinX.Schüwo.SV.Dto;

[UsedImplicitly]
public class UploadOrderPos : IOrderPos
{
    public string oid;

    public int lnbrr { get; set; }

    public int lnbra { get; set; }

    public decimal quant { get; set; }

    public decimal quant_d { get; set; }
        
    public string anbr { get; set; }
        
    public string arts_text_de { get; set; }

    public string unit { get; set; }

    public int price { get; set; }

    public int total { get; set; }

    public decimal vat { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles
