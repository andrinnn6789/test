using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Attendance;

public class AtlasAttendanceListItem
{
    [JsonProperty("ID")]
    public int Id { get; set; }

    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    [JsonProperty("RegistrationUID")]
    public int RegistrationUid { get; set; }

    public double NumberOfDays { get; set; }

    public string Notes { get; set; }
}