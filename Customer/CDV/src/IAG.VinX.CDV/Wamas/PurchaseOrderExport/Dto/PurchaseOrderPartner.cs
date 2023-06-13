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
        PurchaseOrderPartner AS
        (
            SELECT DISTINCT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                'SUPPLIER'                                  AS RoleKind,
                CAST(FLOOR(Adr_Adressnummer) as nvarchar)   AS PartnerId,           
                REPLACE(REPLACE(REPLACE(
                LOWER(Bel_Sprache), 
                    'de', 'ger'), 'fr', 'fra'), 'it', 'it') AS Language,
                'NORMAL'                                    AS RecordState,
                Bel_Datum                                   AS DeliveryTimeSlotFrom          
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID
            JOIN Adresse ON Bel_AdrID = Adr_ID
            JOIN Lager ON Bel_LagerID = Lag_ID
            JOIN Bereich ON Bel_BereichID = Bereich_ID
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
public class PurchaseOrderPartner : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPurchaseOrderPartnerRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string Id { get; set; }

    public string RoleKind { get; set; }

    public string PartnerId { get; set; }

    [NotMapped] public string ExchangePartner { get; set; }

    [NotMapped] public string Title { get; set; }

    [NotMapped] public string Name { get; set; }

    [NotMapped] public string Name2 { get; set; }

    [NotMapped] public string Name3 { get; set; }

    [NotMapped] public string Name4 { get; set; }

    [NotMapped] public string Street { get; set; }

    [NotMapped] public string City { get; set; }

    [NotMapped] public string District { get; set; }

    [NotMapped] public string Region { get; set; }

    [NotMapped] public string Country { get; set; }

    [NotMapped] public string ZipCode { get; set; }

    [NotMapped] public string Phone { get; set; }

    [NotMapped] public string Fax { get; set; }

    [NotMapped] public string Email { get; set; }

    [NotMapped] public string Website { get; set; }

    [NotMapped] public string Gln { get; set; }

    public string Language { get; set; }

    [NotMapped] public string TimeZone { get; set; }

    public string RecordState { get; set; }
    
    [FieldHidden] [FieldConverter(ConverterKind.Date, "yyyyMMddhhmmss")]
    public DateTime DeliveryTimeSlotFrom { get; set; }
}