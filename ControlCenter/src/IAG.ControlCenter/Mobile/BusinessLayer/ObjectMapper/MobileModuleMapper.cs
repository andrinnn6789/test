using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;

public class MobileModuleMapper : ObjectMapper<MobileModule, MobileModule>
{
    protected override MobileModule MapToDestination(MobileModule source, MobileModule destination)
    {
        destination.Id = source.Id;
        destination.TenantId = source.TenantId;
        destination.Licence = source.Licence;
        destination.ModuleName = source.ModuleName;
        destination.LicencedUntil = source.LicencedUntil;

        return destination;
    }
}