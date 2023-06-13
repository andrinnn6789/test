using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;

public class AtlasAttendanceListRegister
{
    public AtlasAttendanceListRegister()
    {
        AttendanceListItems = new List<AtlasAttendanceListItem>();
    }

    [JsonProperty("AttendanceListUID")]
    public int AttendanceListUid { get; set; }

    public string Document { get; set; }

    public DateTime? AttendanceListBegin { get; set; }

    public DateTime? AttendanceListEnd { get; set; }

    public List<AtlasAttendanceListItem> AttendanceListItems { get; set; }
}