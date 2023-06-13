using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class Employment
{
    public string Position { get; set; }

    public string EmployedDate { get; set; }

    public double? Engagement { get; set; }
        
    [JsonProperty("LGAVSubordinated")]
    public string LgavSubordinated { get; set; }

    public string Compensation { get; set; }

    public string ValidFrom { get; set; }
}