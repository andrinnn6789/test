﻿using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class Employer
{
    [JsonProperty("EmployerUID")]
    public int EmployerUid { get; set; }

    public string Name { get; set; }

    public string Name2 { get; set; }

    public string AddressRow1 { get; set; }

    public string AddressRow2 { get; set; }

    [JsonProperty("ZIPCode")]
    public string ZipCode { get; set; }

    public string Location { get; set; }

    [JsonProperty("CountryISO")]
    public string CountryIso { get; set; }
        
    public string EMail { get; set; }

    public string Phone { get; set; }

    [JsonProperty("URL")]
    public string Url { get; set; }
   
    [CanBeNull]
    public Contact Contact { get; set; }
}