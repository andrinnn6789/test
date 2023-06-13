using System;
using System.Xml.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.SalesPdf.Dto;

[TableCte(@"
        WITH Producer (
            Id, Bezeichnung, Text, Geschichte, Land,
            LastChange,
            Logo, Foto1, Foto2, Foto3, Foto4, Foto5, Foto6
        ) AS (
        SELECT DISTINCT
            Prod_Id, TRIM(Prod_Bezeichnung), TRIM(Prod_Text), Prod_Geschichte, TRIM(Land_Bezeichnung),
            GREATER(Prod_ChangedOn, ArtKat_ChangedOn),
            Prod_Logo, Prod_Foto1, Prod_Foto2, Prod_Foto3, Prod_Foto4, Prod_Foto5, Prod_Foto6 
        FROM Produzent
        JOIN Artikel ON Art_ProdId = Prod_Id
        JOIN Artikelkategorie ON Art_AKatID = ArtKat_ID
        JOIN Land ON Art_LandID = Land_Id     
        " + WineInfo.Filter + @"
        )
    ")]
[UsedImplicitly]
public class Producer
{
    public int Id { get; set; }
    public string Bezeichnung { get; set; }
    public string Text { get; set; }
    public string Land { get; set; }
    public string Geschichte { get; set; }
    public DateTime LastChange { get; set; }
    [XmlIgnore]
    public byte[] Logo { get; set; }
    [XmlIgnore]
    public byte[] Foto1 { get; set; }
    [XmlIgnore]
    public byte[] Foto2 { get; set; }
    [XmlIgnore]
    public byte[] Foto3 { get; set; }
    [XmlIgnore]
    public byte[] Foto4 { get; set; }
    [XmlIgnore]
    public byte[] Foto5 { get; set; }
    [XmlIgnore]
    public byte[] Foto6 { get; set; }
}