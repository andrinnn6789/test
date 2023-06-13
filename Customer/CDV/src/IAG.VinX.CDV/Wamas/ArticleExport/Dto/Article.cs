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
    Article AS
    (
         SELECT
            CAST(FLOOR(Art_Artikelnummer) as nvarchar)  AS ItemNumber,                     
            'A' + CAST(FLOOR(Abf_ID) as nvarchar)       AS BaseQuantityUnit,
            CASE  
                WHEN Gross_ID IS NULL THEN
                    'A' + CAST(FLOOR(Abf_ID) as nvarchar) 
                ELSE
                    'G' + CAST(FLOOR(Gross_ID) as nvarchar)  
            END                                         AS WarehouseQuantityUnit,
            CASE Art_Artikeltyp
                    WHEN 2 THEN 'Wein'
                    WHEN 3 THEN 'Min.&Saft'
                    WHEN 4 THEN 'Bier'
                    WHEN 5 THEN 'Spirituose'
                    WHEN 10 THEN 'Food'
                    WHEN 12 THEN 'Non-Food'
                    ELSE 'Diverse'
                END                                     AS ItemType,
            Art_LagerPreis                              AS GoodsValue,
            'CHF'                                       AS GoodsValueUnit,
            UPPER(IsNull(Land_Isocode, 'CH'))           AS OriginCountry,
            CASE ABS(Art_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'UPDATE'
            END                                         AS RecordState,
            Art_ChangedOn                               AS ChangedOn
        FROM Artikel
        JOIN Abfuellung ON Art_AbfID = Abf_ID
        LEFT JOIN Bereich ON Art_BereichID = Bereich_ID
        LEFT JOIN Grossgebinde ON Art_GrossID = Gross_ID AND IsNull(Gross_EinhProGG, 0) > 0 
        LEFT JOIN Adresse ON Art_LieferantNr = Adr_Adressnummer
        LEFT JOIN Land ON Art_LandID = Land_ID
        WHERE 
            ABS(Art_MitLager) = 1 
            AND Art_Artikelnummer IS NOT NULL
            AND (Bereich_Kuerzel IS NULL OR Bereich_Kuerzel = 'Casa')
    )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class Article : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasArticleRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string ItemNumber { get; set; }

    [NotMapped] public string Variant { get; set; } = "2000";

    public string BaseQuantityUnit { get; set; }

    public string WarehouseQuantityUnit { get; set; }

    [NotMapped] public string IsDualItem { get; set; }

    [NotMapped] public string DualQuantityUnit { get; set; }

    [NotMapped] public string DualWarehouseQuantityUnit { get; set; }

    [NotMapped] public string LowerTolerance { get; set; }

    [NotMapped] public string LowerTolerancePercentage { get; set; }

    [NotMapped] public string UpperTolerance { get; set; }

    [NotMapped] public string UpperTolerancePercentage { get; set; }

    [NotMapped] public string AutoConversionMode { get; set; }

    [NotMapped] public string CanEditBaseQuantity { get; set; }

    [NotMapped] public string CanEditDualQuantity { get; set; }

    [NotMapped] public string GoodsCategory { get; set; }

    [NotMapped] public string Assortment { get; set; }

    [NotMapped] public string BillingCategory { get; set; }

    public string ItemType { get; set; }

    public decimal GoodsValue { get; set; }

    public string GoodsValueUnit { get; set; }

    [NotMapped] public string DateStockMode { get; set; }

    [NotMapped] public string BatchStockMode { get; set; }
    
    [NotMapped] public string ProdDateStockMode { get; set; }

    [NotMapped] public string CsiaStockMode01 { get; set; }

    [NotMapped] public string CsiaStockMode02 { get; set; }

    [NotMapped] public string CsiaStockMode03 { get; set; }

    [NotMapped] public string CsiaStockMode04 { get; set; }

    [NotMapped] public string CsiaStockMode05 { get; set; } 

    [NotMapped] public string CsiaStockMode06 { get; set; } 

    [NotMapped] public string CsiaStockMode07 { get; set; } 

    [NotMapped] public string CsiaStockMode08 { get; set; } 

    [NotMapped] public string CsiaStockMode09 { get; set; } 

    [NotMapped] public string CsiaStockMode10 { get; set; } 

    [NotMapped] public string CsiaStockMode11 { get; set; } 

    [NotMapped] public string CsiaStockMode12 { get; set; } 

    [NotMapped] public string CsiaStockMode13 { get; set; } 

    [NotMapped] public string CsiaStockMode14 { get; set; } 

    [NotMapped] public string CsiaStockMode15 { get; set; } 

    [NotMapped] public string CsiaStockMode16 { get; set; } 

    [NotMapped] public string CsiaStockMode17 { get; set; } 

    [NotMapped] public string CsiaStockMode18 { get; set; } 

    [NotMapped] public string CsiaStockMode19 { get; set; } 

    [NotMapped] public string CsiaStockMode20 { get; set; } 

    [NotMapped] public string ItemWeight { get; set; } 

    [NotMapped] public string ItemWeightUnit { get; set; } 

    [NotMapped] public string PackSequence { get; set; } 

    [NotMapped] public string ItemEquivalentPeriod { get; set; } 

    [NotMapped] public string ItemEquivalentPeriodUnit { get; set; } 

    [NotMapped] public string ItemClass { get; set; } 

    [NotMapped] public string TimeBasedAccessMode { get; set; } 

    [NotMapped] public string AutoConversionModeIncoming { get; set; } 

    [NotMapped] public string CanEditBaseQuantityIncoming { get; set; } 

    [NotMapped] public string CanEditDualQuantityIncoming { get; set; } 

    [NotMapped] public string DateRegistrationIncoming { get; set; } 

    [NotMapped] public string BatchRegistrationIncoming { get; set; } 

    [NotMapped] public string ProdDateRegistrationIncoming { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming01 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming02 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming03 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming04 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming05 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming06 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming07 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming08 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming09 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming10 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming11 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming12 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming13 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming14 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming15 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming16 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming17 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming18 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming19 { get; set; } 

    [NotMapped] public string CsiaRegistrationIncoming20 { get; set; } 

    [NotMapped] public string TransferBdl { get; set; } 

    [NotMapped] public string RemLifeCheckIncoming { get; set; } 

    [NotMapped] public string MinRemLifeIncoming { get; set; } 

    [NotMapped] public string MinRemLifeUnitIncoming { get; set; } 

    [NotMapped] public string MaxRemLifeIncoming { get; set; } 

    [NotMapped] public string MaxRemLifeUnitIncoming { get; set; } 

    [NotMapped] public string ItemMonitorKind { get; set; } 

    [NotMapped] public string MonitorInverval { get; set; } 

    [NotMapped] public string MonitorInvervalUnit { get; set; } 

    [NotMapped] public string IsOverDeliveryAllowed { get; set; } 

    [NotMapped] public string GtinControlKind { get; set; } 

    [NotMapped] public string GtinVerification { get; set; } 

    [NotMapped] public string GtinVerificationUnit { get; set; } 

    [NotMapped] public string AutoConversionModeOutgoing { get; set; } 

    [NotMapped] public string CanEditBaseQuantityOutgoing { get; set; } 

    [NotMapped] public string CanEditDualQuantityOutgoing { get; set; } 

    [NotMapped] public string LoadAidId { get; set; } 

    [NotMapped] public string DateRegistrationOutgoing { get; set; } 

    [NotMapped] public string BatchRegistrationOutgoing { get; set; } 

    [NotMapped] public string ProdDateRegistrationOutgoing { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing01 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing02 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing03 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing04 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing05 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing06 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing07 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing08 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing09 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing10 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing11 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing12 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing13 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing14 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing15 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing16 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing17 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing18 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing19 { get; set; } 

    [NotMapped] public string CsiaRegistrationOutgoing20 { get; set; } 

    [NotMapped] public string AvailabilityCheck { get; set; } 

    [NotMapped] public string IsObdBlocked { get; set; } 

    [NotMapped] public string RemLifeCheckOutgoing { get; set; } 

    [NotMapped] public string RemLifeOutgoing { get; set; } 

    [NotMapped] public string RemLifeUnitOutgoing { get; set; } 

    [NotMapped] public string RequiredLoadAidId { get; set; } 

    [NotMapped] public string UpperDeliveryTolerance { get; set; } 

    [NotMapped] public string LowerDeliveryTolerance { get; set; } 

    [NotMapped] public string StackingCategory { get; set; } 

    [NotMapped] public string CustomsTariffNumber { get; set; } 

    public string OriginCountry { get; set; }

    public string RecordState { get; set; }

    [FieldHidden] public DateTime ChangedOn { get; set; }
}