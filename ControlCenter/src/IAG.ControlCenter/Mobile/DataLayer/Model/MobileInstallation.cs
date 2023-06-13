using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Mobile.DataLayer.Model;

public class MobileInstallation : BaseEntityWithTenant
{
    public string Url { get; set; }

    public string Name { get; set; }

    public int SyncInterval { get; set; } = 10;

    /// <summary>
    /// 3 byte RGB-color as string, eg. "#EFDECD"
    /// </summary>
    public string Color { get; set; }
}