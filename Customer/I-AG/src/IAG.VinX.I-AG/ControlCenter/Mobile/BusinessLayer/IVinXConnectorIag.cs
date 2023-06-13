using System;

using IAG.Common.DataLayerSybase;
using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;

namespace IAG.VinX.IAG.ControlCenter.Mobile.BusinessLayer;

public interface IVinXConnectorIag: IBaseSybaseRepository
{
    void UpdateLicence(MobileLicence license);
    LicenceSync GetLicenceConfig(DateTime lastRead);
}