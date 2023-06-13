using System.Collections.Generic;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Registration;

public class RegistrationList
{
    public RegistrationList()
    {
        Header = new Header();
        RegistrationListItems = new List<RegistrationListItem>();
    }

    public Header Header { get; set; }

    public List<RegistrationListItem> RegistrationListItems { get; set; }
}