using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;

public class AtlasRegistrationDocument
{
    [JsonProperty("ID")]
    public int Id { get; set; }

    [JsonProperty("HGfLGAVDatei")]
    public string HgfLgavDatei { get; set; }
}