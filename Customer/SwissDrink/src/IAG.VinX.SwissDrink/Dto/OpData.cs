using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.SwissDrink.Dto;

[TableCte(@"
        WITH OpData AS
        (
           SELECT 
                OP_Id                       AS Id, 
                OP_BelegId                  AS ReceiptId,
                ABS(Adr_RechnungZugferd)    AS HasZugferd
            FROM OP 
            JOIN Adresse ON Adr_Id = OP_AdresseID
        )
        ")]
public class OpData
{
    public int Id { get; [UsedImplicitly] set; }
    public int ReceiptId { get; [UsedImplicitly] set; }
    public bool HasZugferd { get; set; }
}