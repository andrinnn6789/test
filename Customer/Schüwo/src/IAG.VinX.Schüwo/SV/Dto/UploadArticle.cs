using JetBrains.Annotations;

namespace IAG.VinX.Schüwo.SV.Dto;

[UsedImplicitly]
// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class UploadArticle
{
    public string Anbr { get; set; }
    public int Status { get; set; }
    public string Text_De { get; set; }
    public string Origin { get; set; }
    public string Brand { get; set; }
    public string Unit1 { get; set; }
    public int? Price1 { get; set; }
    public int Deposit1 { get; set; }
    public string Unit2 { get; set; }
    public int? Price2 { get; set; }
    public int? Deposit2 { get; set; }
    public string CatId1 { get; set; }
    public string Longtext_De { get; set; }
    public decimal Vat { get; set; }
    public string Minord { get; set; }
    public string Codes1{ get; set; }
    public string Codes2{ get; set; }
}