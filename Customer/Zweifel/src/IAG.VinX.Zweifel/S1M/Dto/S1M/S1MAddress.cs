using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.BaseData.Dto.Sybase;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MAddress AS (
    SELECT
        Station_AdresseId               AS          Id,
        Station_Id                      AS          StationId,
        Adr_Adressnummer                AS          Number,
        Adr_Name                        AS          Name,
        Adr_Strasse                     AS          Street,
        Adr_Ort                         AS          City,
        Adr_PLZ                         AS          Zip,
        Adr_Email                       AS          Email,
        Adr_Zeitfenster1Von             AS          DeliveryTimeFrom1,
        Adr_Zeitfenster1Bis             AS          DeliveryTimeTo1,
        Adr_Zeitfenster2Von             AS          DeliveryTimeFrom2,
        Adr_Zeitfenster2Bis             AS          DeliveryTimeTo2,
        Adr_Zeitfenster3Von             AS          DeliveryTimeFrom3,
        Adr_Zeitfenster3Bis             AS          DeliveryTimeTo3,
        ABS(Adr_Telefonavisierung)      AS          NotifyByTelephone,
        Adr_SchluesselBadgeCode         AS          KeyBadgeCode,
        ABS(Adr_lieferscheinMitPreis)   AS          DeliverySlipWithPrice,
        Adr_LieferKontaktpersonID       AS          ContactPersonId,
        VerrechArt_Bezeichnung          AS          SettlementTypeDesignation
    FROM Station
    JOIN Adresse ON Adresse.Adr_ID = Station_AdresseID
    JOIN Verrechnungsart ON VerrechArt_ID = Adr_VerrechnungsartID
)
")]
public class S1MAddress
{
    public int Id { get; set; }
    public int StationId { get; set; }
    public decimal Number { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Zip { get; set; }
    public string EMail { get; set; }
    public TimeSpan DeliveryTimeFrom1 { get; set; }
    public TimeSpan DeliveryTimeTo1 { get; set; }
    public TimeSpan DeliveryTimeFrom2 { get; set; }
    public TimeSpan DeliveryTimeTo2 { get; set; }
    public TimeSpan DeliveryTimeFrom3 { get; set; }
    public TimeSpan DeliveryTimeTo3 { get; set; }
    public bool NotifyByTelephone { get; set; }
    public string KeyBadgeCode { get; set; }
    public bool DeliverySlipWithPrice { get; set; }

    [JsonIgnore]
    public int ContactPersonId { get; set; }

    [NotMapped]
    [CanBeNull]
    public IEnumerable<ContactPerson> ContactPersons { get; set; }

    public string SettlementTypeDesignation { get; set; }
}