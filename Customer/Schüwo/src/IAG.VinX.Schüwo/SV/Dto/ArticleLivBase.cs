using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH
    ArticleLivBase (
        Artikelnummer,  
        Produzent,  
        Produktionsland, Region, Zutatenliste, DDContent
    )    
    AS
    (
    SELECT DISTINCT
            Art_Artikelnummer,  
            Prod_Bezeichnung, 
            Land_IsoCode, Reg_Bezeichnung, DDProduct_ContentInfo, DDProduct_Content
        FROM Artikel Art
        JOIN Land ON Art.Art_LandID = Land_ID
        JOIN Abfuellung ON Art.Art_AbfID = Abf_ID
        JOIN Grossgebinde ON Art.Art_GrossID = Gross_ID
        JOIN MWST ON Art.Art_MWSTID = MWST_ID
        LEFT OUTER JOIN Artikelkategorie ON Art_AKatID = ArtKat_ID
        LEFT OUTER JOIN Produzent ON Art_ProdId = Prod_ID
        LEFT OUTER JOIN Region ON Art_RegId = Reg_ID
        LEFT OUTER JOIN DDBundle ON Art_DDBundleID = DDBundle_ID
        LEFT OUTER JOIN DDProduct ON DDBundle_DDProductID = DDProduct_ID
        WHERE " + ArticleSw.Filter + @"
    )
    ")]
[UsedImplicitly] 
public class ArticleLivBase
{
    public int Artikelnummer { get; set; }
    public string Produzent { get; set; }
    public string Produktionsland { get; set; }
    public string Region { get; set; }
    public string Zutatenliste { get; set; }
    public string DdContent { get; set; }
}