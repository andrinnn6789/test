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
        PurchaseOrder AS
        (
            SELECT DISTINCT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                LEFT(Lag_Bezeichnung, 20)                   AS WarehouseLocation, 
                Bel_Datum                                   AS DeliveryTimeSlotFrom,
                'NORMAL'                                    AS RecordState                
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID
            JOIN Lager ON Bel_LagerID = Lag_ID
            JOIN Bereich ON Bel_BereichID = Bereich_ID
            JOIN Adresse ON Bel_AdrID = Adr_ID
            WHERE Bel_Belegtyp = -20 
                AND Bel_Belegstatus = 65 
                AND (Bel_Logistikstatus IS NULL OR Bel_Logistikstatus = 60) 
                AND Lag_Bezeichnung = 'Rupperswil'
                AND Bereich_Kuerzel = 'Casa'
                AND Adr_Adressnummer IS NOT NULL
        )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class PurchaseOrder : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPurchaseOrderRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string Id { get; set; }

    public string WarehouseLocation { get; set; }

    [NotMapped] public string Type { get; set; } = "CDV";

    [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime DeliveryTimeSlotFrom { get; set; }

    [NotMapped] public string DeliveryTimeSlotTo { get; set; }

    [NotMapped] public string DeliveryNoteNumber { get; set; }

    [NotMapped] public string DeliveryNoteDate { get; set; }

    [NotMapped] public string Priority { get; set; }

    [NotMapped] public string IncomingArea { get; set; }

    [NotMapped] public string Dock { get; set; }

    [NotMapped] public string StagingArea { get; set; }

    [NotMapped] public string StorageLocation { get; set; }

    public string RecordState { get; set; }
}