using JetBrains.Annotations;
// ReSharper disable UnusedMember.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[UsedImplicitly]
public class UploadOpenOrderPos
{
    public string Anbr { get; set; }

    public string Cid { get; set; }
        
    public decimal Quantity { get; set; }

    public string Unit { get; set; }
}