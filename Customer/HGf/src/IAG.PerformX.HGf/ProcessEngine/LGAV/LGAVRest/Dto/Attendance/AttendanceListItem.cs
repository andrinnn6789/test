using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Attendance;

public class AttendanceListItem
{
    public AttendanceListItem()
    {
        Attendance = new Attendance();
    }

    public string Mode { get; set; }

    [JsonProperty("RegistrationUID")]
    public int RegistrationUid { get; set; }

    public Attendance Attendance { get; set; }
}