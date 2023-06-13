using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

public class AttendanceListRegisterResponseItem
{
    [JsonProperty("AttendanceListUID")]
    public int AttendanceListUid { get; set; }

    [JsonProperty("AttendanceListID")]
    public int AttendanceListId { get; set; }

    public string Operation { get; set; }
}