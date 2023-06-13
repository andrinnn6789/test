using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Smith.BossExport.Dto;

[TableCte(@"
        WITH ArticleBoss (
            Id, Ean, EanPackage, EanContainer, Description, DescriptionShort,
            Structure, VatRate, PriceSell, AgeMin
        ) AS (
        SELECT DISTINCT
            Art.Art_Id, CAST(Art.Art_EAN1 AS BIGINT), CAST(ArtAbf.Art_EAN1 AS BIGINT), CAST(Art.Art_EAN2 AS BIGINT), TRIM(Art.Art_Bezeichnung), TRIM(Art.Art_Suchbegriff),
            Case Art.Art_Artikeltyp 
                WHEN 2 THEN '91.01.9887.05.05 - WEINE' 
                WHEN 4 THEN '91.01.9889.05.05 - BIERE'
                ELSE '91.01.9888.05.05 - SPIRITUOSEN' END, 
            MWST_Prozent, VK_Preis, 
            Case Art.Art_Artikeltyp 
                WHEN 5 THEN 18 
                ELSE 16 END
        FROM Artikel Art 
        JOIN Artikelkategorie ON Art.Art_AKatID = ArtKat_ID
        JOIN MWST ON Art.Art_MWSTID = MWST_ID
        JOIN VKPreis ON VK_ArtId = Art.Art_Id 
        JOIN Abfuellung ON Art.Art_AbfId = Abf_Id
        JOIN ArtikelSelektionscode ON ASelCode_ArtID = Art.Art_Id
        LEFT OUTER JOIN Artikel ArtAbf ON Abf_ArtId = ArtAbf.Art_ID
        WHERE (ISNULL(VK_VonDatum, CURRENT DATE) <= CURRENT DATE) AND (ISNULL(VK_BisDatum, CURRENT DATE) >= CURRENT DATE)
            AND VK_KGrpPreisID = 1 
            AND Art.Art_Artikeltyp IN (2, 4, 5)     -- Wein, Bier, Spirituosen
            AND Art.Art_ZyklusID IN (1, 10, 13)     -- Aktiv, Aktiv Shop, Aktiv LOEB
            AND ASelCode_SelID = 168                -- Bridge
        )
    ")]
public class ArticleBoss
{
    public int Id { get; set; }
    public long Ean { get; set; }
    public long EanPackage { get; set; }
    public long EanContainer { get; set; }
    public string Description { get; set; }
    public string DescriptionShort { get; set; }
    public string Structure { get; set; }
    public decimal VatRate { get; set; }
    public decimal PriceSell { get; set; }
    public int AgeMin { get; set; }
}