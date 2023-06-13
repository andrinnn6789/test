using System;
using System.Collections.Generic;

using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

public class S1MExtDelivery
{
    public int DeliveryId { get; set; }
    public string TourNumber { get; set; }
    public string TourName { get; set; }
    public Vehicle Vehicle { get; set; }
    public string DriverUref { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime DeliveryDate { get; set; }
    public int StartKms { get; set; }
    public int EndKms { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public IEnumerable<S1MHalt> Halts { get; set; }
    public decimal Weight { get; set; }
}