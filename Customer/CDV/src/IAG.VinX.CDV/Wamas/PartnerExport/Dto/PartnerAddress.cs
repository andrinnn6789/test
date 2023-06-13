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
    PartnerAddress AS
    (
         SELECT
            CAST(FLOOR(Adr_Adressnummer) as nvarchar)   AS PartnerId, 
            CASE 
                WHEN Adr_Adressnummer = Adr_RechAdrID
                THEN 'INVOICE'
                ELSE 'PARTNER'
            END                                         AS AddressKind, 
            LEFT(REPLACE(RTRIM(LTRIM(
                COALESCE(Adr_Name + ' ', '') + 
                COALESCE(Adr_Vorname, ''))), '\""', ''), 35)        
                                                        AS Name,
            UPPER(IsNull(Land_Isocode, 'CH'))           AS Country,            
            REPLACE(REPLACE(REPLACE(
                LOWER(Adr_Sprache), 
                    'de', 'ger'), 'fr', 'fra'), 'it', 'it')
                                                        AS Language,
            CASE ABS(Adr_Aktiv)
                WHEN 0 THEN 'DELETE'
                ELSE 'NORMAL'
            END                                         AS RecordState,
            Adr_ChangedOn                               AS ChangedOn
        FROM Adresse
        JOIN Land ON Land.Land_Id = Adresse.Adr_LandId
        WHERE Adr_Adressnummer IS NOT NULL
    )
    ")]
[UsedImplicitly]
[Serializable]
[ExcludeFromCodeCoverage]
[DelimitedRecord(";")]
public class PartnerAddress : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPartnerAddressRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string PartnerId { get; set; }

    public string AddressKind { get; set; }

    [NotMapped] public string Title { get; set; }

    public string Name { get; set; }

    [NotMapped] public string Name2 { get; set; }

    [NotMapped] public string Name3 { get; set; }

    [NotMapped] public string Name4 { get; set; }

    [NotMapped] public string Street { get; set; }

    [NotMapped] public string City { get; set; }

    [NotMapped] public string District { get; set; }

    [NotMapped] public string Region { get; set; }

    public string Country { get; set; }

    [NotMapped] public string ZipCode { get; set; }

    [NotMapped] public string Phone { get; set; }

    [NotMapped] public string Fax { get; set; }

    [NotMapped] public string Email { get; set; }

    [NotMapped] public string Website { get; set; }

    [NotMapped] public string Gln { get; set; }

    public string Language { get; set; }

    [NotMapped] public string TimeZone { get; set; }

    public string RecordState { get; set; }
    
    [FieldHidden] public DateTime ChangedOn { get; set; }
}