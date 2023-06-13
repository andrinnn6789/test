using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.PickListExport.Dto;

[TableCte(@"
    WITH
        PickList AS
        (
            SELECT DISTINCT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                LEFT(Lag_Bezeichnung, 20)                   AS WarehouseLocation, 
                Bel_Datum                                   AS RequestedDeliveryDateFrom,
                IsNull(CAST(Bel_SpedID as nvarchar), '')    AS ShippingMethod,
                'NORMAL'                                    AS RecordState,
                Bel_DatumErfassung                          AS CreateDate
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID AND BelPos_MengeAbf >= 0
            JOIN Lager ON Bel_LagerID = Lag_ID
            JOIN Bereich ON Bel_BereichID = Bereich_ID
            JOIN Adresse ON Bel_AdrID = Adr_ID
            WHERE Bel_Belegtyp = 20 
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
public class PickList : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPickListRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string Id { get; set; }

    public string WarehouseLocation { get; set; }

    [NotMapped] public string Type { get; set; } = "CDV";

    [NotMapped] public string OrderTime { get; set; }

    public string ShippingMethod { get; set; }

    [NotMapped] public string ShippingCategory { get; set; }

    [NotMapped] public string Priority { get; set; }

    [NotMapped] public string IsObdSplitAllowed { get; set; }

    [NotMapped] public string IsReductionAllowed { get; set; }

    [NotMapped] public string RoundingByRules { get; set; }

    public string RecordState { get; set; }

    [FieldHidden] public DateTime RequestedDeliveryDateFrom { get; set; }
    
    [FieldHidden] public DateTime CreateDate { get; set; }
}