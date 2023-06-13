using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class RegistrationListResponseItem
{
    [JsonProperty("RegistrationUID")]
    public int RegistrationUid { get; set; }

    [JsonProperty("RegistrationID")]
    public int RegistrationId { get; set; }

    public string Operation { get; set; }
}