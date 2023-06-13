using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

[TableCte(@"
        WITH FreightCost AS
         (
             SELECT Fracht_ID             AS ID,
                    Fracht_Beschreibung   AS Description,
                    Fracht_KGrpPreisID    AS PriceGroupId,
                    KundPreis_Bezeichnung AS PriceGroupName,
                    Fracht_ArtikelID      AS ArticleId,
                    Art_Bezeichnung       AS ArticleName,
                    Fracht_FrachtfreiAb   AS FreightExemptFrom,
                    Fracht_Frachtkosten   AS FreightPrice,
                    MWST_Prozent          AS TaxRate

             FROM Frachtkosten
                      JOIN KundengruppePreis ON KundPreis_ID = Fracht_KGrpPreisID
                      JOIN Artikel ON Art_ID = Fracht_ArtikelID
                      JOIN MWST ON Mwst_ID = Art_MWSTID
         )
    ")]
public class FreightCost
{
    public int Id { get; set; }
    public int PriceGroupId { get; set; }
    public string PriceGroupName { get; set; }
    public int ArticleId { get; set; }
    public string ArticleName { get; set; }
    public decimal FreightExemptFrom { get; set; }
    public decimal FreightPrice { get; set; }
    public decimal TaxRate { get; set; }
}