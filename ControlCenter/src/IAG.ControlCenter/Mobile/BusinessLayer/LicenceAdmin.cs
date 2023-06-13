using System.Collections.Generic;
using System.Linq;

using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Mobile.DataLayer.Context;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.DataLayer.ObjectMapper;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.ControlCenter.Mobile.BusinessLayer;

public class LicenceAdmin
{
    private readonly MobileDbContext _context;
    private readonly UserDatabaseAuthenticationPlugin _userAuthPlugin;
    private List<MobileLicence> _changedLicences;

    public const string AppUserRoleName = "AppUser";

    public LicenceAdmin(MobileDbContext context, UserDatabaseAuthenticationPlugin userAuthPlugin)
    {
        _context = context;
        _userAuthPlugin = userAuthPlugin;
    }

    public List<MobileLicence> SyncSystems(LicenceSync remoteSystems)
    {
        _changedLicences = new List<MobileLicence>();
        UpdateSystems(remoteSystems);
        _context.RemoveDeletedEntries(remoteSystems.Tenants);
        _context.RemoveDeletedEntries(remoteSystems.Installations);
        _context.RemoveDeletedEntries(remoteSystems.Licences);
        _context.RemoveDeletedEntries(remoteSystems.Modules);
        _context.SaveChanges();

        return _changedLicences;
    }

    public void UpdateSystems(LicenceSync remoteSystems)
    {
        AddAndUpdate(remoteSystems);
        SyncLicencesToUsers(remoteSystems.LicenceUsers);
        _context.SaveChanges();
    }

    private void SyncLicencesToUsers(List<User> licenceUsers)
    {
        foreach (var user in licenceUsers)
        {
            _userAuthPlugin.AddOrUpdateUser(
                user,
                AppUserRoleName,
                ClaimNamesInfrastructure.GeneralClaim,
                ScopeNamesInfrastructure.ReaderScope,
                PermissionKind.Read
            );
        }
    }

    private void AddAndUpdate(LicenceSync remoteSystems)
    {
        _context.SyncLocalEntities(remoteSystems.Tenants, new TenantMapper());
        _context.SyncLocalEntities(remoteSystems.Installations, new InstallationMapper());
        _context.SyncLocalEntities(remoteSystems.Modules, new MobileModuleMapper());

        foreach (var licence in remoteSystems.Licences)
        {
            var localLicence = _context.MobileLicences.FirstOrDefault(s => s.Id.Equals(licence.Id));
            if (localLicence == null)
            {
                localLicence = new MobileLicence
                {
                    Id = licence.Id, 
                    TenantId = licence.TenantId,
                    LicenceStatus = LicenceStatusEnum.New
                };
                _context.MobileLicences.Add(localLicence);
            }

            localLicence.Licence = licence.Licence;

            switch (licence.LicenceStatus)
            {
                case LicenceStatusEnum.Reset:
                    localLicence.Licence = licence.Licence;
                    localLicence.DeviceInfo = licence.DeviceInfo;
                    localLicence.DeviceId = licence.DeviceId;
                    localLicence.LicenceStatus = LicenceStatusEnum.New;
                    licence.LicenceStatus = LicenceStatusEnum.New;
                    _changedLicences?.Add(licence);
                    break;
                case LicenceStatusEnum.Revoked:
                    localLicence.LicenceStatus = LicenceStatusEnum.Revoked;
                    break;
                default:
                    if (licence.Licence != localLicence.Licence || licence.DeviceInfo != localLicence.DeviceInfo ||
                        licence.DeviceId != localLicence.DeviceId || licence.LicenceStatus != localLicence.LicenceStatus)
                    {
                        licence.Licence = localLicence.Licence;
                        licence.DeviceInfo = localLicence.DeviceInfo;
                        licence.DeviceId = localLicence.DeviceId;
                        licence.LicenceStatus = localLicence.LicenceStatus;
                        _changedLicences?.Add(licence);
                    }
                    break;
            }
        }
    }
}