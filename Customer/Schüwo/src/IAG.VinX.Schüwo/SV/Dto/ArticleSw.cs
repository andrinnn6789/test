using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH
    Sortenanteil (ArtZus_WeinInfoID, ArtZus_Position, Bezeichnung) AS 
        (
        SELECT ArtZus_WeinInfoID, ArtZus_Position, 
        ' ' + CASE ISNULL(ArtZus_Anteil, -1) WHEN -1 THEN '' ELSE TRIM(str(ArtZus_Anteil)) + '% ' END + Sorte_Bezeichnung
                            FROM ArtikelZusammensetzung
                            JOIN Traubensorte ON ArtZus_TraubensorteID = Sorte_ID 
        ),
    ArtikelSorten (AS_ID, AS_Sorten) AS 
        (
        SELECT Art_ID, TRIM(LIST(Bezeichnung ORDER BY ArtZus_Position))
                            FROM Sortenanteil
                            JOIN WeinInfo ON ArtZus_WeinInfoID = Wein_ID
                            JOIN Artikel ON Art_WeinInfoID = Wein_ID 
                            GROUP BY Art_ID          
                            ORDER BY Art_ID
        ),
    ArticleSw (
        ArtNr, Description, CountryIso, BrandId, FillingInCl, 
        FillingTextShort, UnitsPerBulkPackaging, BulkPackagingTextShort, 
        IsTank,      
        SellAsUnit, ArtEGroupId, 
        MainArtEGroupId, 
        Price, 
        DepositPriceUnit, DepositPriceContainer, 
        Vat, Vintage, Grapes, 
        Terroir, Vinification, Charakter, ConsumationHint, Rating,
        Barrique, EanUnit, 
        EanBulk
    )
    AS
    (
        SELECT DISTINCT
            Art.Art_Artikelnummer, Art.Art_Bezeichnung, Land_IsoCode, Art.Art_MarkeID, Abf_InhaltInCl, 
            Abf_KuerzelWeb, Gross_EinhProGG, Gross_KuerzelWeb, 
            CASE IsNull(Gross_Verpackungstyp, 0) WHEN 30 THEN 1 ELSE 0 END * CASE IsNull(Abf_InhaltInCl, 0) WHEN 100 THEN 1 ELSE 0 END,
            ABS(Gross_AnbruchErlaubtOnline), Art.Art_EGruppeID, 
            COALESCE(eGruppeParentParent.ArtEGrp_ID, eGruppeParent.ArtEGrp_ID, eGruppe.ArtEGrp_ID),
            ISNULL((COALESCE(VkSpiga.VK_Preis, VkSV.VK_Preis, VkGastro.VK_Preis)) * 100, 0),
            GebArtikelAbf.Art_Grundpreis * 100, GebArtikelGG.Art_Grundpreis * 100,
            MWST_Prozent / 100, Convert(Varchar, Art.Art_Jahrgang), AS_Sorten,
            Wein_Terroir, Wein_Vinifikation, Wein_Charakter, Wein_Konsumhinweis, Wein_Bewertung,
            Art.Art_Barriqueausbau, CAST(CAST(Art.Art_EAN1 AS BIGINT) AS VARCHAR), 
            CASE Art.Art_EAN2 WHEN Art.Art_EAN1 THEN NULL ELSE CAST(CAST(Art.Art_EAN2 AS BIGINT) AS VARCHAR) END
        FROM Artikel Art
        JOIN Abfuellung ON Art.Art_AbfID = Abf_ID
        JOIN Grossgebinde ON Art.Art_GrossID = Gross_ID
        JOIN MWST ON Art.Art_MWSTID = MWST_ID
        LEFt OUTER JOIN Land ON Art.Art_LandID = Land_ID
        LEFT OUTER JOIN ArtikelEGruppe eGruppe ON Art.Art_EGruppeID = eGruppe.ArtEGrp_ID
        LEFT OUTER JOIN ArtikelEGruppe eGruppeParent ON eGruppeParent.ArtEGrp_ID = eGruppe.ArtEGrp_ObergruppeID
        LEFT OUTER JOIN ArtikelEGruppe eGruppeParentParent ON eGruppeParentParent.ArtEGrp_ID = eGruppeParent.ArtEGrp_ObergruppeID
        LEFT OUTER JOIN VKPreis VkSpiga ON Art.Art_ID = VkSpiga.VK_ArtID AND VkSpiga.VK_KGrpPreisID = 20
        LEFT OUTER JOIN VKPreis VkSV ON Art.Art_ID = VkSV.VK_ArtID AND VkSV.VK_KGrpPreisID = 10
        LEFT OUTER JOIN VKPreis VkGastro ON Art.Art_ID = VkGastro.VK_ArtID AND VkGastro.VK_KGrpPreisID = 11
        LEFT OUTER JOIN Artikel GebArtikelAbf ON Abf_ArtID = GebArtikelAbf.Art_ID
        LEFT OUTER JOIN Artikel GebArtikelGG ON Gross_ArtID = GebArtikelGG.Art_ID
        LEFT OUTER JOIN WeinInfo ON Art.Art_WeininfoID = Wein_ID
        LEFT OUTER JOIN ArtikelSorten ON AS_ID = Art.Art_ID
        WHERE " + Filter + @"
    )
    ")]
[UsedImplicitly]
public class ArticleSw
{
    public const string Filter = @"Art.Art_Aktiv = -1 AND Art.Art_SVServiceSparteIDNeu IS NOT NULL AND Art.Art_Artikelnummer IS NOT NULL
                                    AND (ISNULL(VkSpiga.VK_VonDatum, CURRENT DATE) <= CURRENT DATE) AND (ISNULL(VkSpiga.VK_BisDatum, CURRENT DATE) >= CURRENT DATE)
                                    AND (VkSpiga.VK_TarifstufeID = 1 OR VkSpiga.VK_TarifstufeID IS NULL)
                                    AND (ISNULL(VkSV.VK_VonDatum, CURRENT DATE) <= CURRENT DATE) AND (ISNULL(VkSV.VK_BisDatum, CURRENT DATE) >= CURRENT DATE)
                                    AND (VkSV.VK_TarifstufeID = 1 OR VkSV.VK_TarifstufeID IS NULL)
                                    AND (ISNULL(VkGastro.VK_VonDatum, CURRENT DATE) <= CURRENT DATE) AND (ISNULL(VkGastro.VK_BisDatum, CURRENT DATE) >= CURRENT DATE) 
                                    AND (VkGastro.VK_TarifstufeID = 1 OR VkGastro.VK_TarifstufeID IS NULL)";

    public int ArtNr { get; set; }
    public string Description { get; set; }
    public string CountryIso { get; set; }
    public int? BrandId { get; set; }
    public decimal? FillingInCl { get; set; }
    public string FillingTextShort { get; set; }
    public int UnitsPerBulkPackaging { get; set; }
    public string BulkPackagingTextShort { get; set; }
    public bool SellAsUnit { get; set; }
    public int? ArtEGroupId { get; set; }
    public int Price { get; set; }
    public int DepositPriceUnit { get; set; }
    public int DepositPriceContainer { get; set; }
    public decimal Vat { get; set; }
    public string Vintage{ get; set; }
    public string Charakter{ get; set; }
    public string Grapes{ get; set; }
    public string Terroir { get; set; }
    public string Vinification{ get; set; }
    public string ConsumationHint{ get; set; }
    public string Rating{ get; set; }
    public int Barrique { get; set; }
    public bool IsTank { get; set; }
    public string EanUnit{ get; set; }
    public string EanBulk{ get; set; }

    public int Price1 => SellAsUnit ? Price : Price * UnitsPerBulkPackaging;
}