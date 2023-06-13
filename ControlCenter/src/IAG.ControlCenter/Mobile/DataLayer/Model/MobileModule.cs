using System;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Mobile.DataLayer.Model;

public class MobileModule : BaseEntityWithTenant
{
    public string Licence { get; set; }
    
    public string ModuleName { get; set; }

    public DateTime? LicencedUntil { get; set; }

}