using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

using FileHelpers;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.ArticleExport.Dto;

[TableCte(@"
    WITH
    ArticleAlias AS
    (
        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            CAST(FLOOR(Art_EAN1) as nvarchar)           AS ItemAliasNumber,
            'A' + CAST(FLOOR(Art_AbfID) as nvarchar)    AS QuantityUnit,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel 
        JOIN Abfuellung ON Art_AbfID = Abf_ID
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE IsNull(Art_EAN1, 0) <> 0 
            AND ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')

        UNION ALL

        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            CAST(FLOOR(Art_EAN2) as nvarchar)           AS ItemAliasNumber,
             'G' + CAST(FLOOR(Art_GrossID) as nvarchar) AS QuantityUnit,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE IsNull(Art_EAN2, 0) <> 0 
            AND ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')      

        UNION ALL

        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            CAST(FLOOR(Art_EAN3) as nvarchar)           AS ItemAliasNumber,
             'G' + CAST(FLOOR(Art_GrossID) as nvarchar) AS QuantityUnit,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE IsNull(Art_EAN3, 0) <> 0 
            AND ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')
    )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class ArticleAlias : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasArticleAliasRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string ItemNumber { get; set; }

    [NotMapped] public string Variant { get; set; } = "2000";

    public string ItemAliasNumber { get; set; }

    [NotMapped] public string Kind { get; set; } = "GTIN";

    public string QuantityUnit { get; set; }

    [NotMapped] public string NumberOfBaseAndWarehouseQuantityUnit { get; set; }

    public string RecordState { get; set; }
    
    [FieldHidden] public DateTime ChangedOn { get; set; }
}