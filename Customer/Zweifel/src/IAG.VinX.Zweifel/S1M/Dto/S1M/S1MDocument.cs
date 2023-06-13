using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Formatter;

using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MDocument AS (
    SELECT
        Bel_ID                          AS  Id,
        Bel_BelegNr                     AS  Number,
        Bel_IRef                        AS  IRef,
        Bel_Hinweis                     AS  RemarkRtf,
        Bel_Belegtyp                    AS  Type,
        Bel_BestellDatum                AS  OrderDate,
        Bel_AdrID                       AS  AddressId,
        Mit_URef                        AS  Uref,
        ABS(Bel_HatAdHocLieferadresse)  AS  HasAdHocDeliveryAddress,
        Bel_AdHocAdresse                AS  AdHocAddress,
        Bel_AdHocStrasse                AS  AdHocStreet,
        Bel_AdHocPLZ                    AS  AdHocZip,
        Bel_AdHocOrt                    AS  AdHocCity,
        Bel_AdHocLand                   AS  AdHocCountry,
        Bel_AdHocTel                    AS  AdHocTelephone
    FROM Beleg
    JOIN Mitarbeiter ON Mit_ID = Bel_MitarbeiterID
)
")]
public class S1MDocument
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string IRef { get; set; }

    [JsonIgnore]
    public string RemarkRtf { get; set; }

    public string Remark => RtfCleaner.Clean(RemarkRtf);
    public int Type { get; set; }
    public DateTime OrderDate { get; set; }
    public string Uref { get; set; }
    public bool HasAdHocDeliveryAddress { get; set; }
    public string AdHocStreet { get; set; }
    public string AdHocZip { get; set; }
    public string AdHocCity { get; set; }
    public string AdHocCountry { get; set; }
    public string AdHocTelephone { get; set; }
    [JsonIgnore]
    public int AddressId { get; set; }

    [NotMapped]
    public IEnumerable<S1MArticlePosition> ArticlePositions { get; set; }

    [NotMapped]
    public IEnumerable<S1MBulkPackagePosition> BulkPackagePositions { get; set; }
}