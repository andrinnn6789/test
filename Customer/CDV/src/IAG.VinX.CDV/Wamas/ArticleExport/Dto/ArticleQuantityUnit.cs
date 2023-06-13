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
    ArticleQuantityUnit AS
    (
        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)                      AS ItemNumber,
            1                                                               AS Denominator,
            'A' + CAST(FLOOR(Art_AbfID) as nvarchar)                        AS QuantityUnit,
            '1'                                                             AS Numerator,
            'A' + CAST(FLOOR(Art_AbfID) as nvarchar)                        AS ReferenceQuantityUnit,
            IsNull(CAST(Art_Breite as nvarchar), '')                        AS 'Length',    
            CASE WHEN Art_Breite IS NULL 
                THEN '' ELSE 'cm' END                                       AS LengthUnit,
            IsNull(CAST(Art_Hoehe as nvarchar), '')                         AS Height,
            CASE WHEN Art_Hoehe IS NULL 
                THEN '' ELSE 'cm' END                                       AS HeightUnit,
            IsNull(CAST(Art_Tiefe as nvarchar), '')                         AS Width,
            CASE WHEN Art_Tiefe IS NULL 
                THEN '' ELSE 'cm' END                                       AS WidthUnit,
            IsNull(CAST(Art_Breite * Art_Hoehe
                     * Art_Tiefe as nvarchar), '')                          AS Volume,
            CASE WHEN CAST(Art_Breite * Art_Hoehe
                     * Art_Tiefe as nvarchar) IS NULL
                THEN '' ELSE 'cm3' END                                      AS VolumeUnit,           
            IsNull(CAST(FLOOR(Art_Gewichtsanteil * 1000) as nvarchar), '')  AS GrossWeight,
            'gr'                                                            AS GrossWeightUnit,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                                             AS RecordState,
            Art_ChangedOn                                                   AS ChangedOn           
        FROM Artikel
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')

        UNION ALL

        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            1                                           AS Denominator,
            'G' + CAST(FLOOR(Art_GrossID) as nvarchar)  AS QuantityUnit,
             CAST(FLOOR(Gross_EinhProGG) as nvarchar)   AS Numerator,
            'A' + CAST(FLOOR(Art_AbfID) as nvarchar)    AS ReferenceQuantityUnit,
            ''                                          AS 'Length',    
            ''                                          AS LengthUnit,
            ''                                          AS Height,
            ''                                          AS HeightUnit,
            ''                                          AS Width,
            ''                                          AS WidthUnit,
            ''                                          AS Volume,
            ''                                          AS VolumeUnit,
            ''                                          AS GrossWeight,
            ''                                          AS GrossWeightUnit,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel
        JOIN Grossgebinde ON Art_GrossID = Gross_ID
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE ABS(Art_MitLager) = 1  
            AND IsNull(Gross_EinhProGG, 0) > 0 
            AND Art_Artikelnummer IS NOT NULL 
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')
    )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class ArticleQuantityUnit : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasArticleQuantityUnitRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string ItemNumber { get; set; }

    [NotMapped] public string Variant { get; set; } = "2000";

    public int Denominator { get; set; }

    public string QuantityUnit { get; set; }

    public string Numerator { get; set; }

    public string ReferenceQuantityUnit { get; set; }

    public string GrossWeight { get; set; }

    public string GrossWeightUnit { get; set; }

    public string Length { get; set; }

    public string LengthUnit { get; set; }

    public string Width { get; set; }

    public string WidthUnit { get; set; }

    public string Height { get; set; }

    public string HeightUnit { get; set; }

    public string Volume { get; set; }

    public string VolumeUnit { get; set; }

    [NotMapped] public string StackingFactor { get; set; }

    [NotMapped] public string WholeLoadingUnit { get; set; }

    [NotMapped] public string LoadingUnitCubature { get; set; }

    [NotMapped] public string EmptiesItemNumber { get; set; }

    [NotMapped] public string EmptiesVariant { get; set; }

    [NotMapped] public string IsSalesQuantity { get; set; }

    public string RecordState { get; set; }
    
    [FieldHidden] public DateTime ChangedOn { get; set; }
}