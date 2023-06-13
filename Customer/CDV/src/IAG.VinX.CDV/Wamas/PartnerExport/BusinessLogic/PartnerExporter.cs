using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PartnerExport.Dto;

namespace IAG.VinX.CDV.Wamas.PartnerExport.BusinessLogic;

public class PartnerExporter : BaseExporter, IPartnerExporter
{
    public PartnerExporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }
    
    protected override Type[] RecordTypes => new[] { typeof(Partner), typeof(PartnerAddress) };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasExportJobResult ExportPartner(DateTime lastSync)
    {
        var jobResult = new WamasExportJobResult();

        try
        {
            var records = GetRecords(lastSync);

            if (records.Any())
                SerializeAndUpload(records, ResourceIds.WamasPartnerRecordType);

            jobResult.Result = JobResultEnum.Success;
            jobResult.ExportedCount = records.Count;
        }
        catch (Exception e)
        {
            jobResult.Result = JobResultEnum.Failed;
            jobResult.ErrorCount++;
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasPartnerExportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasPartnerExportErrorTitle,
                string.Format(ResourceIds.WamasPartnerExportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private List<GenericWamasRecord> GetRecords(DateTime lastSync)
    {
        var records = new List<GenericWamasRecord>();
        var partners = DatabaseConnector.GetQueryable<Partner>().Where(p => p.ChangedOn >= lastSync).ToList();
        var partnerAddresses = DatabaseConnector.GetQueryable<PartnerAddress>().Where(p => p.ChangedOn >= lastSync).ToList();

        foreach (var partner in partners)
        {
            records.Add(new GenericWamasRecord(partner.GetType(), partner));

            var addressesForPartner = partnerAddresses.Where(p => p.PartnerId == partner.PartnerId).ToList();
            foreach (var partnerAddress in addressesForPartner)
            {
                records.Add(new GenericWamasRecord(partnerAddress.GetType(), partnerAddress));
            }
        }

        return records;
    }
}