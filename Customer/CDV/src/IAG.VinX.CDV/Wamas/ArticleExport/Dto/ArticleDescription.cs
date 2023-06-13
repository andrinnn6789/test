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
    ArticleDescription AS
    (
        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            LEFT(REPLACE(REPLACE(REPLACE(REPLACE(
                Art_Suchbegriff, '\""', ''), ';', ''),
                CHAR(13), ''), CHAR(10), ''), 40)       AS Description,
            LEFT(REPLACE(
                CASE 
                    WHEN Art_Jahrgang IS NOT NULL THEN
                     CAST(FLOOR(Art_Jahrgang) as nvarchar) 
                       + ' / ' 
                       + Prod_Bezeichnung
                    ELSE Prod_Bezeichnung
                END , '\""', ''), 40)                   AS Description2,        
            'ger'                                       AS Language,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel
        JOIN Produzent ON Art_ProdID = Prod_ID
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE Art_Suchbegriff IS NOT NULL 
            AND ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')
        
        UNION ALL

        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            LEFT(REPLACE(REPLACE(REPLACE(REPLACE(
                Art_ProduktTitelFR, '\""', ''), ';', ''),
                CHAR(13), ''), CHAR(10), ''), 40)       AS Description,
            LEFT(REPLACE(
                CASE 
                    WHEN Art_Jahrgang IS NOT NULL THEN
                     CAST(FLOOR(Art_Jahrgang) as nvarchar) 
                       + ' / ' 
                       + Prod_Bezeichnung
                    ELSE Prod_Bezeichnung
                END , '\""', ''), 40)                   AS Description2,
            'fra'                                       AS Language,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel  
        JOIN Produzent ON Art_ProdID = Prod_ID
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE Art_ProduktTitelFR IS NOT NULL 
            AND ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')

        UNION ALL

        SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,
            LEFT(REPLACE(REPLACE(REPLACE(REPLACE(
                Art_ProduktTitelIT, '\""', ''), ';', ''),
                CHAR(13), ''), CHAR(10), ''), 40)       AS Description,
            LEFT(REPLACE(
                CASE 
                    WHEN Art_Jahrgang IS NOT NULL THEN
                     CAST(FLOOR(Art_Jahrgang) as nvarchar) 
                       + ' / ' 
                       + Prod_Bezeichnung
                    ELSE Prod_Bezeichnung
                END , '\""', ''), 40)                   AS Description2,
            'it'                                        AS Language,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel
        JOIN Produzent ON Art_ProdID = Prod_ID
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        WHERE Art_ProduktTitelIT IS NOT NULL 
            AND ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')
    )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class ArticleDescription : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasArticleDescriptionRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string ItemNumber { get; set; }

    [NotMapped] public string Variant { get; set; } = "2000";

    public string Description { get; set; }

    public string Description2 { get; set; }

    public string Language { get; set; }

    public string RecordState { get; set; }
    
    [FieldHidden] public DateTime ChangedOn { get; set; }
}