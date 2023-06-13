using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class Registration
{
    public Registration()
    {
        Participant = new Participant();
        Employment = new Employment();
        Documents = new List<string>();
    }

    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    public Participant Participant { get; set; }

    [CanBeNull]
    public Employer Employer { get; set; }

    public Employment Employment { get; set; }

    public List<string> Documents { get; set; }

    public string Notes { get; set; }
}