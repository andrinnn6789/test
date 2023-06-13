using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;
using IAG.Infrastructure.DataLayer.Model.System;

namespace IAG.VinX.IAG.ControlCenter.Mobile.BusinessLayer;

public class VinXConnectorIag : BaseSybaseRepository, IVinXConnectorIag
{
    public VinXConnectorIag(ISybaseConnection sybaseConnection) : base(sybaseConnection)
    {
    }

    #region public

    public void UpdateLicence(MobileLicence license)
    {
        const string sql = @"UPDATE MobileLicence SET Mol_LicenceStatus= ?,  Mol_DeviceId = ?, Mol_DeviceInfo = ? WHERE Mol_GUID = ?";
        using var cmd = SybaseConnection.CreateCommand(sql, 
            license.LicenceStatus,
            license.DeviceId,
            license.DeviceInfo,
            license.Id);
        cmd.ExecuteNonQuery();
    }

    public LicenceSync GetLicenceConfig(DateTime lastRead)
    {
        var config = new LicenceSync
        {
            Tenants = SybaseConnection.QueryBySql<Tenant>(@"
                    SELECT 
                        Adr_GUID as Id,
                        Adr_Suchbegriff as Name
                    FROM
                        Adresse
                    WHERE 
                        Adr_ChangedOn > ? AND
                        EXISTS (SELECT 1 FROM MobileLicence WHERE Mol_AdresseID = Adr_Id) OR 
                        EXISTS (SELECT 1 FROM MobileInstallation WHERE Mot_AdresseID = Adr_Id)", 
                lastRead).ToList(),
            Licences = SybaseConnection.QueryBySql<MobileLicence>(@"
                    SELECT
                        Mol_Guid as Id, 
                        Mol_Key as Licence, 
                        Mol_LicenceStatus as LicenceStatus, 
                        Mol_DeviceInfo as DeviceInfo, 
                        Mol_DeviceID as DeviceID, 
                        Adr_GUID as TenantId
                    FROM
                        MobileLicence 
                    JOIN Adresse on Mol_AdresseID = Adr_Id
                    WHERE
                        Mol_ChangedOn > ?",
                lastRead).ToList(),
            LicenceUsers = SybaseConnection.QueryBySql<User>(@"
                    SELECT 
                        Mol_Key as Name, 
                        Mol_Key as Password, 
                        Adr_Sprache as Culture
                    FROM
                        MobileLicence 
                    JOIN Adresse on Mol_AdresseID = Adr_Id
                    WHERE
                        Mol_ChangedOn > ?",
                lastRead).ToList(),
            Installations = SybaseConnection.QueryBySql<MobileInstallation>(@"
                    SELECT 
                        Mot_Guid as Id, 
                        Mot_Url as Url, 
                        Mot_Name as Name,
                        CASE WHEN Mot_Color is Null THEN Null ELSE '#' + SUBSTR(CAST(INTTOHEX(Mot_Color) as VARCHAR), 3) END as Color,
                        Adr_GUID as TenantId
                    FROM
                        MobileInstallation 
                    JOIN Adresse on Mot_AdresseID = Adr_Id
                    WHERE
                        Mot_ChangedOn > ?",
                lastRead).ToList(),
            Modules = SybaseConnection.QueryBySql<MobileModule>(@"
                    SELECT 
                        App_Key as Id,
                        Mol_Key as Licence, 
                        App_AppModul as ModuleName, 
                        App_LicensedUntil as LicencedUntil
                    FROM
                        LicencedApp
                    JOIN
                        MobileLicence ON App_MobileLicenceID = Mol_ID").ToList()
        };
        return config;
    }

    #endregion
}