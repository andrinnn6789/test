using System.Collections.Generic;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

public class AttendanceListRegisterItem
{
    public AttendanceListRegisterItem()
    {
        AttendanceListItems = new List<AttendanceListItem>();
        AttendanceListDates = new AttendanceListDates();
        Documents = new List<string>();
    }
     
    public string Mode { get; set; }

    [JsonProperty("AttendanceListUID")]
    public int AttendanceListUid { get; set; }

    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    public AttendanceListDates AttendanceListDates { get; set; }

    public List<AttendanceListItem> AttendanceListItems { get; set; }

    public List<string> Documents { get; set; }
}