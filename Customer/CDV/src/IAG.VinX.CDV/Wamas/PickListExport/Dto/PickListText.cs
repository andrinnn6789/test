﻿using System;
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
        PickListText AS
        (
            SELECT DISTINCT
                CAST(FLOOR(Bel_ID) as nvarchar)             AS Id, 
                'VinX-Belegnummer'                          AS KeyName,
                CAST(Bel_BelegNr as nvarchar)               AS Number,
                Bel_Datum                                   AS RequestedDeliveryDateFrom,
                REPLACE(REPLACE(RTRIM(LTRIM(
                    COALESCE(Bel_Hinweis, '')))
                    , '\""', ''), ';', '') AS Text,
                'NORMAL'                                    AS RecordState,
                Bel_DatumErfassung                          AS CreateDate   
            FROM Beleg
            JOIN ArtikelPosition ON Bel_ID = BelPos_BelegID
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
public class PickListText : IWamasRecord
{
    [NotMapped] public string Source { get; set; } = "VINX";

    [NotMapped] public string Target { get; set; } = "WAMAS";

    [NotMapped] public int SerialNumber { get; set; }

    [NotMapped]
    [FieldConverter(ConverterKind.Date, "yyyyMMddHHmmss")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    [NotMapped] public string DatasetType { get; set; } = ResourceIds.WamasPickListTextRecordType;

    [NotMapped] public string ClientId { get; set; } = "CDV";

    public string Id { get; set; }

    [NotMapped] public string TextTypeClientId { get; set; } = "CDV";

    [NotMapped] public string TextType { get; set; } = "WA-Auftrag";

    [NotMapped] public string Language { get; set; } = "ger";

    [NotMapped] public string Sequence { get; set; } = "1";
    
    public string Text { get; set; }

    public string RecordState { get; set; }
    
    [FieldHidden] public DateTime RequestedDeliveryDateFrom { get; set; }
    
    [FieldHidden] public DateTime CreateDate { get; set; }
}