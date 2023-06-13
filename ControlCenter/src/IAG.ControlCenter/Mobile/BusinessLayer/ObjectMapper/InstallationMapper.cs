using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;

public class InstallationMapper : ObjectMapper<MobileInstallation, MobileInstallation>
{
    protected override MobileInstallation MapToDestination(MobileInstallation source, MobileInstallation destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.Url = source.Url;
        destination.TenantId = source.TenantId;
        destination.Color = source.Color;
        return destination;
    }
}