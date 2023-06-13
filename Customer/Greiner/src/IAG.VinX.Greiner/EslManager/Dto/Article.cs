using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Greiner.EslManager.Dto;

[TableCte(@"
    WITH
    Article (
        ArtId, ArtNr, Description, VatFactor, Price,
        PromotionPrice, Category,
        ArticleGroup, Deposit,
        Content, PackageName, PackageContent, PackageDeposit, TaxRate
    )
    AS
    (
        SELECT
            Art.Art_Id, Art.Art_Artikelnummer, TRIM(SUBSTR(Art.Art_Bezeichnung, 0, 255)),
            Vat_Factor = CASE
                             WHEN KundPreis_InklMWST = -1 THEN 1
                             ELSE ((MWST_Prozent / 100) + 1)
                           END,
            Preis = Round(20 * Vat_Factor * VK_Preis, 0) / 20,
            PreisAktion = CASE
                                 WHEN CURRENT DATE BETWEEN VK_DatumVon AND VK_DatumBis THEN (Round(20 * VK_PreisAktion * Vat_Factor, 0) / 20)
                                 ELSE 0
                             END,
            SUBSTR(ArtKat_Bezeichnung, 6),
            CASE Art.Art_Artikeltyp
                    WHEN 2 THEN 'Wein'
                    WHEN 3 THEN 'Mineral und Fruchtsäfte'
                    WHEN 4 THEN 'Bier'
                    WHEN 5 THEN 'Spirituosen'
                    WHEN 10 THEN 'Food'
                    WHEN 12 THEN 'Non-Food'
                    ELSE 'Diverse'
                END, ArtAbf.Art_Grundpreis,
            Abf_Kuerzel, Gross_Kuerzel, Gross_EinhProGG, ArtGross.Art_Grundpreis, MWST_Prozent
        FROM Artikel Art
            JOIN MWST ON MWST_ID = Art.Art_MWSTID
            JOIN VKPreis ON Art.Art_ID = VK_ArtID
            JOIN KundenGruppePreis ON VK_KGrpPreisID = KundPreis_ID
            LEFT OUTER JOIN Artikelkategorie ON ArtKat_ID = Art.Art_AKatID
            LEFT OUTER JOIN Abfuellung ON Abf_ID = Art.Art_AbfID
            LEFT OUTER JOIN Artikel ArtAbf ON ArtAbf.Art_ID = Abf_ArtId
            LEFT OUTER JOIN Grossgebinde ON Gross_ID = Art.Art_GrossID
            LEFT OUTER JOIN Artikel ArtGross ON ArtGross.Art_ID = Gross_ArtId
        WHERE Art.Art_Aktiv = -1 AND Art.Art_Artikeltyp NOT IN (15, 1, 20)
            AND VK_KGrpPreisID = 17
    )
    ")]
public class Article
{
    public int ArtId { get; set; }
    public int ArtNr { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    public string Category { get; set; }
    public string ArticleGroup { get; set; }
    public decimal Deposit { get; set; }
    public string Content { get; set; }
    public string PackageName { get; set; }
    public decimal PackageContent { get; set; }
    public decimal PackageDeposit { get; set; }
    public decimal TaxRate { get; set; }
}