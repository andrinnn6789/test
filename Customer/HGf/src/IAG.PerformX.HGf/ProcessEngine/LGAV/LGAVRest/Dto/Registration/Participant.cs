using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class Participant
{
    [JsonProperty("PersonUID")]
    public int PersonUid { get; set; }

    public string Name { get; set; }

    public string Name2 { get; set; }

    public string Firstname { get; set; }

    public string Title { get; set; }

    public string AddressRow1 { get; set; }

    public string AddressRow2 { get; set; }

    [JsonProperty("ZIPCode")]
    public string ZipCode { get; set; }

    public string Location { get; set; }

    [JsonProperty("CountryISO")]
    public string CountryIso { get; set; }

    public string DateOfBirth { get; set; }

    public string Gender { get; set; }
        
    public string SocialSecurityNumber { get; set; }
}