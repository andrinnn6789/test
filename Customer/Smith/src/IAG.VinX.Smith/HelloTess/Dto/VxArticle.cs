using IAG.Infrastructure.Atlas;
using IAG.VinX.Smith.HelloTess.SyncLogic;

using JetBrains.Annotations;

namespace IAG.VinX.Smith.HelloTess.Dto;

[UsedImplicitly]
public class VxArticle : IKeyable
{
    public const string Select = @"
            WITH ArticleBase (
                Id, Guid, Bezeichnung, Artikelnummer, ArtikelkategorieGuid,
                EanProEinheit, MwstProzent, InhousePreis
            ) AS (
            SELECT 
                Art_Id, Art_Guid, TRIM(Art_Bezeichnung), CAST(Art_Artikelnummer AS INT), ArtKat_GUID,  
                CAST(Art_Ean1 as BIGINT), MWST_Prozent, vk_Preis
            FROM Artikel
            JOIN VKPreis on VK_ArtID = Art_ID
            JOIN Artikelkategorie ON Art_AKatID = ArtKat_ID
            JOIN Land ON Art_LandID = Land_Id        
            JOIN Mwst ON Art_MWSTID = Mwst_ID
            WHERE Art_Aktiv = -1 AND Art_Ean1 IS NOT NULL AND VK_KGrpPreisID = 17
            ),
            Price (
                ArtId, PriceGroupId, Price
            ) AS ( 
            SELECT 
                VK_ArtID, VK_KGrpPreisID, VK_Preis
            FROM VKPreis
            JOIN ArticleBase ON VK_ArtID = ArticleBase.Id
            WHERE (ISNULL(VK_VonDatum, CURRENT DATE) <= CURRENT DATE) AND (ISNULL(VK_BisDatum, CURRENT DATE) >= CURRENT DATE)
                AND VK_KGrpPreisID = ?
            ORDER BY VK_BisDatum DESC
            ),
            PriceSpec (
                ArtId, CustomerGroupId, Price
            ) AS ( 
            SELECT  
                VKSpez_ArtID, VKSpez_KundID, VKSpez_Preis
            FROM VKSpezPreis
            JOIN ArticleBase ON VKSpez_ArtID = ArticleBase.Id
            WHERE (ISNULL(VKSpez_DatumVon, CURRENT DATE) <= CURRENT DATE) AND (ISNULL(VKSpez_DatumBis, CURRENT DATE) >= CURRENT DATE)
                 AND VKSpez_KundID = ?
            ORDER BY VKSpez_DatumBis DESC
            ),
            VxArticle (
                Id, Guid, Bezeichnung, Artikelnummer, ArtikelkategorieGuid,
                EanProEinheit, MwstProzent, InhousePreis, 
                PriceGroupId, CustomerGroupId, PriceGroupPrice
            ) AS (
            SELECT Id, Guid, Bezeichnung, Artikelnummer, ArtikelkategorieGuid,
                EanProEinheit, MwstProzent, InhousePreis, 
                PriceGroupId, CustomerGroupId, IsNull(IsNull(PriceSpec.Price, Price.Price), 0)
            FROM ArticleBase
            LEFT OUTER JOIN Price ON Price.ArtID = ArticleBase.Id
            LEFT OUTER JOIN PriceSpec ON PriceSpec.ArtID = ArticleBase.Id
            )
            SELECT * FROM VxArticle";

    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public byte[] Guid { get; set; }

    public string Bezeichnung { get; set; }

    public int Artikelnummer { get; set; }
        
    public byte[] ArtikelkategorieGuid { get; set; }

    public long EanProEinheit { get; set; }

    public decimal InhousePreis { get; set; }
        
    public decimal PriceGroupPrice { get; set; }

    public decimal MwstProzent { get; set; }

    public int PriceGroupId { get; set; }

    public int CustomerGroupId { get; set; }
        
    public string Key => new GuidConverter().ToBigEndianGuid(Guid).ToString();
}