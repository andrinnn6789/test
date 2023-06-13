using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH
    ArchiveOrder (
        ExtId, StatusPayment, OrderNr, CustomerNr, ChangedOn, OrderDate
    )
    AS
    (
        SELECT 
            Bel.Bel_ExterneID, FolgeBel.Bel_Belegstatus,
            Bel.Bel_BelegNr, Adr_Adressnummer, Bel.Bel_DatumMutation, Bel.Bel_Datum
        FROM Beleg Bel
        JOIN Adresse ON Bel.Bel_AdrID = Adr_ID
        JOIN Beleg FolgeBel ON Bel.Bel_FolgeBelegID = FolgeBel.Bel_ID
        WHERE Adr_KKatID = 8 AND Adr_Aktiv = -1 AND Bel.Bel_Belegtyp = 30 AND Bel.Bel_Belegstatus = 100
    )
    ")]
[UsedImplicitly]
public class ArchiveOrder
{
    public string ExtId { get; set; }
    public int StatusPayment { get; set; }
    public int OrderNr { get; set; }
    public int CustomerNr { get; set; }
    public DateTime? ChangedOn { get; set; }
    public DateTime OrderDate { get; set; }
}