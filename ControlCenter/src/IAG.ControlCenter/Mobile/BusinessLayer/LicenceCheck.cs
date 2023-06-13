using System.Collections.Generic;
using System.Linq;

using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.BusinessLayer.ObjectMapper;
using IAG.ControlCenter.Mobile.DataLayer.Context;
using IAG.ControlCenter.Mobile.DataLayer.Model;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Mobile.BusinessLayer;

public class LicenceCheck
{
    private readonly MobileDbContext _context;

    public LicenceCheck(MobileDbContext context)
    {
        _context = context;
    }

    public LicenceResponse Check(LicenceRequest request)
    {
        var licResponse = new LicenceResponse { LicenceStatus = LicenceStatusAppEnum.Invalid };
        var lic = _context.MobileLicences.FirstOrDefault(l => l.Licence == request.Licence);
        if (lic == null)
            return licResponse;

        switch (lic.LicenceStatus)
        {
            case LicenceStatusEnum.New:
                lic.LicenceStatus = LicenceStatusEnum.Inuse;
                UpdateLicence(lic, request);
                CompleteResponse(lic, licResponse);
                break;
            case LicenceStatusEnum.Inuse:
                if (lic.DeviceId != request.DeviceId)
                    return licResponse;
                UpdateLicence(lic, request);
                CompleteResponse(lic, licResponse);
                break;
            case LicenceStatusEnum.Revoked:
                licResponse.LicenceStatus = LicenceStatusAppEnum.Revoked;
                break;
        }

        _context.SaveChanges();
        return licResponse;
    }

    public LicenceFreeResponse Free(LicenceRequest request)
    {
        var lic = _context.MobileLicences.FirstOrDefault(
            l => l.Licence == request.Licence && l.DeviceId == request.DeviceId && l.LicenceStatus == LicenceStatusEnum.Inuse);
        if (lic == null)
            return new LicenceFreeResponse
            {
                LicenceStatus = LicenceStatusAppEnum.Invalid
            };

        lic.DeviceId = null;
        lic.DeviceInfo = null;
        lic.LicenceStatus = LicenceStatusEnum.New;
        _context.SaveChanges();

        return new LicenceFreeResponse
        {
            LicenceStatus = LicenceStatusAppEnum.New
        };
    }

    private void CompleteResponse(MobileLicence lic, LicenceResponse response)
    {
        response.Backends = new List<Backend>();
        foreach (var installation in _context.MobileInstallation.AsNoTracking()
                     .Where(i => i.TenantId.Equals(lic.TenantId)))
        {
            response.Backends.Add(new Backend
            {
                Id = installation.Id,
                Name = installation.Name,
                Url = installation.Url,
                SyncInterval = installation.SyncInterval,
                Color = installation.Color
            });
        }

        response.LicenceStatus = LicenceStatusAppEnum.Inuse;
        response.TenantId = lic.TenantId;
        response.Modules = _context.MobileModules
            .Where(m => m.Licence.Equals(lic.Licence))
            .AsEnumerable()
            .Select(LicenceModuleMapper.MapToModule).ToList();
    }

    private void UpdateLicence(MobileLicence lic, LicenceRequest request)
    {
        lic.DeviceId = request.DeviceId;
        lic.DeviceInfo = request.DeviceInfo;
    }
}