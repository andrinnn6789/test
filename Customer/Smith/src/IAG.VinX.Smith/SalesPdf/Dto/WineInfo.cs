using System;
using System.Xml.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.SalesPdf.Dto;

[TableCte(@"
        WITH WineInfo (
            Id, ArtikelNummer, Bezeichnung, Land, Region, Produzent,
            Jahrgang, Lagerfaehigkeit, Trinktemperatur, Vinifikation, Terroir, 
            Geschichte, Charakter,
            Traubensorte,
            LastChange,
            Aktiv, Foto1, Foto2, Foto3
        ) AS (
        SELECT
            Art_Id, TRIM(STR(Art_Artikelnummer)), TRIM(Art_Bezeichnung), TRIM(Land_Bezeichnung), TRIM(Reg_Bezeichnung), TRIM(Prod_Bezeichnung),
            TRIM(STR(Art_Jahrgang)), TRIM(STR(Wein_Lagerdauer)), TRIM(STR(Wein_Trinktemparatur)) + '°C', Wein_Vinifikation, Wein_Terroir, 
            Wein_Geschichte, Wein_Charakter,
            (SELECT LIST(Sorte_Bezeichnung + ' ' + TRIM(STR(ArtZus_Anteil)) + '%') FROM Traubensorte JOIN ArtikelZusammensetzung ON ArtZus_TraubensorteID = Sorte_Id AND ArtZus_WeininfoID = Wein_Id),
            GREATER(GREATER(GREATER(Art_ChangedOn, Wein_ChangedOn), Prod_ChangedOn), ArtKat_ChangedOn),
            CASE Art_ZyklusId WHEN 1 THEN -1 ELSE 0 END, 
            Art_Foto, Art_Foto2, Art_Foto3
        FROM Artikel
        JOIN Land ON Art_LandID = Land_Id
        JOIN Produzent ON Art_ProdId = Prod_Id 
        JOIN Artikelkategorie ON Art_AKatID = ArtKat_ID
        LEFT OUTER JOIN Region ON Art_RegId = Reg_Id 
        LEFT OUTER JOIN WeinInfo ON Art_WeininfoId = Wein_Id 
        " + Filter + @"
        )
    ")]
[UsedImplicitly]
public class WineInfo
{
    public const string Filter = @"
            WHERE LEN(TRIM(Prod_Bezeichnung)) > 0 AND Art_ZyklusID IN (1) AND ArtKat_VerkaufsmappeAktiv = -1
            ";

    public int Id { get; set; }
    public string ArtikelNummer { get; set; }
    public string Bezeichnung { get; set; }
    public string Land { get; set; }
    public string Region { get; set; }
    public string Produzent { get; set; }
    public string Jahrgang { get; set; }
    public string Traubensorte { get; set; }
    public string Lagerfaehigkeit { get; set; }
    public string Trinktemperatur { get; set; }
    public string Vinifikation { get; set; }
    public string Terroir { get; set; }
    public string Geschichte { get; set; }
    public string Charakter { get; set; }
    public DateTime LastChange { get; set; }

    [XmlIgnore]
    public AtlasBoolean Aktiv { get; set; }
    [XmlIgnore]
    public byte[] Foto1 { get; set; }
    [XmlIgnore]
    public byte[] Foto2 { get; set; }
    [XmlIgnore]
    public byte[] Foto3 { get; set; }
}