using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming

namespace IAG.VinX.Schüwo.SV.Dto;

[UsedImplicitly]
public class UploadCustomer
{
    public string Cid { get; set; }
    public string Ocid { get; set; }
    public int Status { get; set; }
    public string Name { get; set; }
    public string Addr1 { get; set; }
    public string Addr2 { get; set; }
    public string Zip { get; set; }
    public string Loc { get; set; }
    public string Tel { get; set; }
    public string Fax { get; set; }
    public string Country { get; set; }
    public string Cost_Centre_Id { get; set; }
    public string Channel_Filter { get; set; }
    public string Tour { get; set; }
    public string Otime { get; set; }
}