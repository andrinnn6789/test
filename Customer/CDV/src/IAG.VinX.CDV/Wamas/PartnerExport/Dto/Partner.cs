using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.PartnerExport.Dto;

[TableCte(@"
    WITH
    Partner AS
    (
        SELECT
            CAST(FLOOR(Adr_Adressnummer) as nvarchar)   AS PartnerId, 
            ABS(Adr_Lieferant)                          AS IsSupplier,
            ABS(Adr_Lieferant)^1                        AS IsCustomer,
            0                                           AS IsFreightCarrier,
            ABS(Adr_LieferungErlaubt)^1                 AS IsDeliveryBlocked,
            CASE ABS(Adr_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Adr_ChangedOn                               AS ChangedOn         
        FROM Adresse
        WHERE Adr_Adressnummer IS NOT NULL
    )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class Partner : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPartnerRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string PartnerId { get; set; }

    [NotMapped] public string Category { get; set; }

    public int IsSupplier { get; set; }

    public int IsCustomer { get; set; }

    public int IsFreightCarrier { get; set; }

    [NotMapped] public string VatId { get; set; }

    [NotMapped] public string LoadingAccount { get; set; }

    public int IsDeliveryBlocked { get; set; }

    [NotMapped] public string DifferenceCode { get; set; }

    [NotMapped] public string SplitAllowed { get; set; }

    [NotMapped] public string ReductionAllowed { get; set; }

    [NotMapped] public string GoodsOwnerReplacementKind { get; set; }

    [NotMapped] public string StockRatingReplacementKind { get; set; }

    [NotMapped] public string ItemReplacementKind { get; set; }

    [NotMapped] public string AccessStrategy { get; set; }

    [NotMapped] public string EqualPeriodStrategy { get; set; }

    [NotMapped] public string WholeWarehouseQuantityUnits { get; set; }

    [NotMapped] public string MaxLuWeight { get; set; }

    [NotMapped] public string MaxLuWeightUnit { get; set; }

    [NotMapped] public string MaxLuHeight { get; set; }

    [NotMapped] public string MaxLuHeightUnit { get; set; }

    [NotMapped] public string RoundingByRules { get; set; }

    [NotMapped] public string OverDeliveryAllowed { get; set; }

    public string RecordState { get; set; }

    [FieldHidden] public DateTime ChangedOn { get; set; }
}