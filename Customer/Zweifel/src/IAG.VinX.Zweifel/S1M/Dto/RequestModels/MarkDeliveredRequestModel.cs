using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace IAG.VinX.Zweifel.S1M.Dto.RequestModels;

public class MarkDeliveredRequestModel
{
    public int StartKms { get; set; }
    public int EndKms { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    [CanBeNull] public IEnumerable<Breakage> Breakages { get; set; }
    [CanBeNull] public IEnumerable<Return> Returns { get; set; }
}