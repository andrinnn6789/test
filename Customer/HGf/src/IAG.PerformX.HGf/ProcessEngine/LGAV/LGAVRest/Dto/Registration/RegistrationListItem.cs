using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class RegistrationListItem
{
    public RegistrationListItem()
    {
        Registration = new Registration();
    }

    public string Mode { get; set; }

    [JsonProperty("RegistrationUID")]
    public int RegistrationUid { get; set; }

    public string RegistrationDate { get; set; }

    public Registration Registration { get; set; }
}