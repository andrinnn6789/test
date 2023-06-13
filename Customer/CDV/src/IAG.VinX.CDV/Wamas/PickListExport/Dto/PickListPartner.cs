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
        PickListPartner AS
        (
            SELECT DISTINCT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                'FINAL_CONSIGNEE'                           AS RoleKind,
                ''                                          AS ExchangePartner,
                CAST(FLOOR(Adr_Adressnummer) as nvarchar)   AS PartnerId,
                LEFT(REPLACE(RTRIM(LTRIM(
                    COALESCE(Adr_Name + ' ', '') + 
                    COALESCE(Adr_Vorname, ''))), '\""', ''), 35)        
                                                            AS Name,          
                REPLACE(REPLACE(REPLACE(
                LOWER(Bel_Sprache), 
                    'de', 'ger'), 'fr', 'fra'), 'it', 'it') AS Language,
                Adr_PLZ                                     AS ZipCode,
                Adr_Ort                                     AS City,
                'NORMAL'                                    AS RecordState,
                Bel_Datum                                   AS RequestedDeliveryDateFrom,
                Bel_DatumErfassung                          AS CreateDate   
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID
            JOIN Adresse ON Adr_ID = CASE WHEN Bel_LieferAdresseID is null THEN Bel_AdrID ELSE Bel_LieferAdresseID END
            JOIN Lager ON Bel_LagerID = Lag_ID            
            JOIN Bereich ON Bel_BereichID = Bereich_ID
            WHERE Bel_Belegtyp = 20 
                AND Bel_Belegstatus = 65 
                AND (Bel_Logistikstatus IS NULL OR Bel_Logistikstatus = 60)
                AND Lag_Bezeichnung = 'Rupperswil'
                AND Bereich_Kuerzel = 'Casa'
                AND Adr_Adressnummer IS NOT NULL

            UNION ALL

             SELECT DISTINCT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                'CONTRACTEE'                                AS RoleKind,
                'NO'                                        AS ExchangePartner,
                CAST(FLOOR(Adr_Adressnummer) as nvarchar)   AS PartnerId,
                LEFT(REPLACE(RTRIM(LTRIM(
                    COALESCE(Adr_Name + ' ', '') + 
                    COALESCE(Adr_Vorname, ''))), '\""', ''), 35)        
                                                            AS Name,                
                REPLACE(REPLACE(
                    LOWER(Bel_Sprache), 
                        'de', 'ger'), 'fr', 'fra')          AS Language,
                Adr_PLZ                                     AS ZipCode,
                Adr_Ort                                     AS City,   
                'NORMAL'                                    AS RecordState,
                Bel_Datum                                   AS RequestedDeliveryDateFrom,
                Bel_DatumErfassung                          AS CreateDate   
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID
            JOIN Adresse ON Bel_AdrID = Adr_ID
            JOIN Lager ON Bel_LagerID = Lag_ID
            JOIN Bereich ON Bel_BereichID = Bereich_ID
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
public class PickListPartner : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPickListPartnerRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string Id { get; set; }

    public string RoleKind { get; set; }

    public string PartnerId { get; set; }

    public string ExchangePartner { get; set; }

    [NotMapped] public string Title { get; set; }

    public string Name { get; set; }

    [NotMapped] public string Name2 { get; set; }

    [NotMapped] public string Name3 { get; set; }

    [NotMapped] public string Name4 { get; set; }

    [NotMapped] public string Street { get; set; }

    public string City { get; set; }

    [NotMapped] public string District { get; set; }

    [NotMapped] public string Region { get; set; }

    [NotMapped] public string Country { get; set; }

    public string ZipCode { get; set; }

    [NotMapped] public string Phone { get; set; }

    [NotMapped] public string Fax { get; set; }

    [NotMapped] public string Email { get; set; }

    [NotMapped] public string Website { get; set; }

    [NotMapped] public string Gln { get; set; }

    public string Language { get; set; }

    [NotMapped] public string TimeZone { get; set; }

    public string RecordState { get; set; }

    [FieldHidden] public DateTime RequestedDeliveryDateFrom { get; set; }
    
    [FieldHidden] public DateTime CreateDate { get; set; }
}