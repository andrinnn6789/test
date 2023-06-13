using System;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;

public class AtlasAttendanceResult
{
    [JsonProperty("HGfLGAVBestaetigung")]
    public DateTime HgfLgavBestaetigung { get; set; }

    [JsonProperty("HGfLGAVID")]
    public int HgfLgavId { get; set; }
}