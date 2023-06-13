using System.Collections.Generic;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class RegistrationListResponseMainObject
{
    public RegistrationListResponseMainObject()
    {
        RegistrationListResponseItems = new List<RegistrationListResponseItem>();
    }

    public List<RegistrationListResponseItem> RegistrationListResponseItems { get; set; }
}