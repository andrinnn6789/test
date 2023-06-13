using System;
using System.ComponentModel.DataAnnotations;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.BaseData.Dto.Enums;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MDelivery AS (
    SELECT
        Auslieferung_Id                 AS  DeliveryId,
        Tour_Bezeichnung                AS  TourNumber,
        Tour_Beschreibung               AS  TourName,
        FZ_ID                           AS  VehicleId,
        Mit_Uref                        AS  DriverUref,
        Auslieferung_Status             AS  Status,
        Auslieferung_Datum              AS  DeliveryDate,
        Auslieferung_Anfangskilometer   AS  StartKms,
        Auslieferung_Endkilometer       AS  EndKms,
        Auslieferung_Anfangszeit        AS  StartTime,
        Auslieferung_Endzeit            AS  EndTime
    FROM Auslieferung
    JOIN Tour ON Tour_ID = Auslieferung_TourID
    JOIN Fahrzeug ON FZ_ID = Auslieferung_FahrzeugID
    JOIN Mitarbeiter ON Mit_ID = Auslieferung_ChauffeurID
)
")]
public class S1MDelivery
{
    [Key]
    public int DeliveryId { get; set; }

    public string TourNumber { get; set; }
    public string TourName { get; set; }
    public int VehicleId { get; set; }
    public string DriverUref { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime DeliveryDate { get; set; }
    public int StartKms { get; set; }
    public int EndKms { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}