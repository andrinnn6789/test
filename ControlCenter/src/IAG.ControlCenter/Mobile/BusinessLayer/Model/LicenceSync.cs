using System.Collections.Generic;

using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.DataLayer.Model.System;

namespace IAG.ControlCenter.Mobile.BusinessLayer.Model;

public class LicenceSync
{
    public List<Tenant> Tenants { get; set; } = new();

    public List<MobileLicence> Licences { get; set; } = new();

    public List<User> LicenceUsers { get; set; } = new();

    public List<MobileInstallation> Installations { get; set; } = new();
        
    public List<MobileModule> Modules { get; set; } = new();
}