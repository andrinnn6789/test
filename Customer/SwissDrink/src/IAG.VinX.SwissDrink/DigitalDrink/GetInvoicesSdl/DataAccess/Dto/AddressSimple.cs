using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.BaseData.Dto.Enums;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

[TableCte(@"
        WITH AddressSimple AS 
        (
            SELECT 
                Adr_Id                          AS Id,
                Adr_GLN                         AS Gln,
                Adr_Adressnummer                AS Number,
                Adr_MWSTVerrechnung             AS VatCalculation,
                Adr_ZahlkondID                  AS PaymentConditionId,
                Adr_KGrpPreisID                 AS PriceGroupId,
                Zahlkond_TageNetto              AS PaymentDays,
                Adr_Sprache                     AS Language,
                VerrechArt_ZahlungswegID        AS PaymentMeansSellId,
                VerrechArt_ZahlungswegIDEinkauf AS PaymentMeansPurchaseId
            FROM Adresse
            JOIN Verrechnungsart ON Adr_VerrechnungsartID = VerrechArt_ID
            JOIN Zahlungskondition ON ZahlKond_Id = Adr_ZahlkondID
        ) 
    ")]
public class AddressSimple
{
    public int Id { get; set; }
    public decimal Gln { get; set; }
    public decimal Number { get; set; }
    public VatCalculationType VatCalculation { get; set; }
    public int PaymentConditionId { get; set; }
    public int PriceGroupId { get; set; }
    public int PaymentDays { get; set; }
    public string Language { get; set; }
    public int PaymentMeansSellId { get; set; }
    public int PaymentMeansPurchaseId { get; set; }
}