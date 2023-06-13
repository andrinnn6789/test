using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Mobile.DataLayer.Model;

public class MobileLicence : BaseEntityWithTenant
{
    public string Licence { get; set; }

    public LicenceStatusEnum LicenceStatus { get; set; }

    public string DeviceInfo { get; set; }

    public string DeviceId { get; set; }
}