using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.VinX.BaseData.Dto.Enums;

namespace IAG.VinX.Zweifel.S1M.Dto.Sybase;

[Table("Auslieferung")]
public class ZweifelDelivery
{
    [Key] [Column("Auslieferung_ID")] public int Id { get; set; }
    [Column("Auslieferung_Status")] public DeliveryStatus Status { get; set; }

    [Column("Auslieferung_Anfangskilometer")]
    public int StartKms { get; set; }

    [Column("Auslieferung_Endkilometer")] public int EndKms { get; set; }
    [Column("Auslieferung_Anfangszeit")] public DateTime StartTime { get; set; }
    [Column("Auslieferung_Endzeit")] public DateTime EndTime { get; set; }
}