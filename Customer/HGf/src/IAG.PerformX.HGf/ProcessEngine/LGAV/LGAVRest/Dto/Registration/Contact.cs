using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class Contact
{
    [JsonProperty("ContactID")]
    public int ContactId { get; set; }

    public string Name { get; set; }

    public string Firstname { get; set; }

    public string Title { get; set; }

    public string EMail { get; set; }

    public string Phone { get; set; }
}