using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.PurchaseOrderExport.Dto;

[TableCte(@"
   WITH
        PurchaseOrderLine AS
        (
            SELECT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                CAST(FLOOR(BelPos_ID) as nvarchar)          AS LineId,
                CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ArticleNumber,
                BelPos_MengeAbf                             AS OrderedQuantity,
                Bel_Datum                                   AS DeliveryTimeSlotFrom        
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID
            JOIN Artikel ON BelPos_ArtikelID = Art_ID
            JOIN Abfuellung ON Art_AbfID = Abf_ID
            JOIN Lager ON Bel_LagerID = Lag_ID
            JOIN Bereich ON Bel_BereichID = Bereich_ID
            JOIN Adresse ON Bel_AdrID = Adr_ID
            WHERE Bel_Belegtyp = -20 
                AND Bel_Belegstatus = 65 
                AND (Bel_Logistikstatus IS NULL OR Bel_Logistikstatus = 60)
                AND Lag_Bezeichnung = 'Rupperswil'
                AND Bereich_Kuerzel = 'Casa'
                AND Adr_Adressnummer IS NOT NULL
                AND BelPos_MengeAbf >= 0
                AND ABS(Art_MitLager) = 1
            ORDER BY Bel_ID, BelPos_ID
        )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class PurchaseOrderLine : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPurchaseOrderLineRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string Id { get; set; }

    public string LineId { get; set; }

    public string ArticleNumber { get; set; }

    [NotMapped] public string Variant { get; set; } = "2000";

    [NotMapped] public string NumQuantityUnit { get; set; }

    [NotMapped] public string StockGroup { get; set; }

    [NotMapped] public string StockRating { get; set; }

    [NotMapped] public string GoodsOwner { get; set; } = "CDV";

    [NotMapped] public string Batch { get; set; }

    [NotMapped] public string BestByDate { get; set; }

    [NotMapped] public string ProductionDate { get; set; }

    [NotMapped] public string ReservationNumber { get; set; }

    [NotMapped] public string CrossDockingNumber { get; set; }

    [NotMapped] public string Csia01 { get; set; }

    [NotMapped] public string Csia02 { get; set; }

    [NotMapped] public string Csia03 { get; set; }

    [NotMapped] public string Csia04 { get; set; }

    [NotMapped] public string Csia05 { get; set; }

    [NotMapped] public string Csia06 { get; set; }

    [NotMapped] public string Csia07 { get; set; }

    [NotMapped] public string Csia08 { get; set; }

    [NotMapped] public string Csia09 { get; set; }

    [NotMapped] public string Csia10 { get; set; }

    [NotMapped] public string Csia11 { get; set; }

    [NotMapped] public string Csia12 { get; set; }

    [NotMapped] public string Csia13 { get; set; }

    [NotMapped] public string Csia14 { get; set; }

    [NotMapped] public string Csia15 { get; set; }

    [NotMapped] public string Csia16 { get; set; }

    [NotMapped] public string Csia17 { get; set; }

    [NotMapped] public string Csia18 { get; set; }

    [NotMapped] public string Csia19 { get; set; }

    [NotMapped] public string Csia20 { get; set; }

    [NotMapped] public string OrderedLuId { get; set; }

    public decimal OrderedQuantity { get; set; }

    [NotMapped] public string DeliveryToleranceUpper { get; set; }

    [NotMapped] public string DeliveryToleranceLower { get; set; }

    [NotMapped] public string LineType { get; set; }
    
    [FieldHidden] [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime DeliveryTimeSlotFrom { get; set; }
}