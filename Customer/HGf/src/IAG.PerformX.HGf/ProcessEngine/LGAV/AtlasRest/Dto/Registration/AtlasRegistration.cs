using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Registration;

public class AtlasRegistration
{
    public AtlasRegistration()
    {
        Participant = new List<AtlasParticipant>();
        Employer = new List<AtlasEmployer>();
        Contact = new List<AtlasContact>();
        Documents = new List<AtlasRegistrationDocument>();
    }

    [JsonProperty("RegistrationUID")]
    public int RegistrationUid { get; set; }

    public DateTime? RegistrationDate { get; set; }

    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    public double? Engagement { get; set; }

    public DateTime? EmployedDate { get; set; }

    [JsonProperty("LGAVSubordinated")]
    public bool LgavSubordinated { get; set; }

    public bool Compensation { get; set; }

    public string Notes { get; set; }

    public DateTime? ValidFrom { get; set; }

    public List<AtlasParticipant> Participant { get; set; }

    public List<AtlasEmployer> Employer { get; set; }

    public List<AtlasContact> Contact { get; set; }

    public List<AtlasRegistrationDocument> Documents { get; set; }
}