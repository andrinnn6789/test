using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;

namespace IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;

public static class LicenceModuleMapper
{
    public static Module MapToModule(MobileModule mobileModule)
    {
        return new Module
        {
            Name = mobileModule.ModuleName,
            ValidUntil = mobileModule.LicencedUntil
        };
    }
}