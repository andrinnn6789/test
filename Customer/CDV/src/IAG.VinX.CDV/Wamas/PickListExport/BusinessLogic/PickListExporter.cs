using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Formatter;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PickListExport.Dto;

namespace IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;

public class PickListExporter : BaseExporter, IPickListExporter
{
    private List<PickList> _pickListsToExport = new();
    
    public PickListExporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }

    protected override Type[] RecordTypes => new[]
    {
        typeof(PickList),
        typeof(PickListReference),
        typeof(PickListLine),
        typeof(PickListText),
        typeof(PickListPartner)
    };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasExportJobResult ExportPickLists(DateTime exportUntil, DateTime minimumLeadDate)
    {
        var jobResult = new WamasExportJobResult();

        try
        {
            var records = GetRecords(exportUntil, minimumLeadDate);

            if (records.Any())
            {
                SerializeAndUpload(records, ResourceIds.WamasPickListRecordType);
                var updateResult = UpdateLogisticStates(LogisticState.TransmittedToLogistics);
                
                jobResult.ExportedCount = records.Count;
                jobResult.ErrorCount = updateResult.ErrorCount;
            }

            jobResult.Result = jobResult.ErrorCount == 0 && jobResult.ExportedCount >= 0
                ? JobResultEnum.Success
                : JobResultEnum.PartialSuccess;
        }
        catch (Exception e)
        {
            jobResult.ErrorCount++;
            jobResult.Result = JobResultEnum.Failed;
            UpdateLogisticStates(LogisticState.ErrorTryAgain);

            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasPickListExportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasPickListExportErrorTitle,
                string.Format(ResourceIds.WamasPickListExportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private List<GenericWamasRecord> GetRecords(DateTime exportUntil, DateTime minimumLeadDate)
    {
        var records = new List<GenericWamasRecord>();
        _pickListsToExport = DatabaseConnector.GetQueryable<PickList>()
            .Where(p => p.RequestedDeliveryDateFrom <= exportUntil && p.CreateDate <= minimumLeadDate).ToList();
        var pickListReferences = DatabaseConnector.GetQueryable<PickListReference>()
            .Where(p => p.RequestedDeliveryDateFrom <= exportUntil && p.CreateDate <= minimumLeadDate).ToList();
        var pickListLines = DatabaseConnector.GetQueryable<PickListLine>()
            .Where(p => p.RequestedDeliveryDateFrom <= exportUntil && p.CreateDate <= minimumLeadDate).ToList();
        var pickListTexts = DatabaseConnector.GetQueryable<PickListText>()
            .Where(p => p.RequestedDeliveryDateFrom <= exportUntil && p.CreateDate <= minimumLeadDate).ToList();
        var pickListPartners = DatabaseConnector.GetQueryable<PickListPartner>()
            .Where(p => p.RequestedDeliveryDateFrom <= exportUntil && p.CreateDate <= minimumLeadDate).ToList();

        foreach (var pickList in _pickListsToExport)
        {
            records.Add(new GenericWamasRecord(pickList.GetType(), pickList));

            var referenceForPickList = pickListReferences.First(p => p.Id == pickList.Id);
            records.Add(new GenericWamasRecord(referenceForPickList.GetType(), referenceForPickList));

            var linesForPickList = pickListLines.Where(p => p.Id == pickList.Id).ToList();
            foreach (var pickListLine in linesForPickList)
            {
                records.Add(new GenericWamasRecord(pickListLine.GetType(), pickListLine));
            }
            
            var textsForPickList = pickListTexts.Where(p => p.Id == pickList.Id && !string.IsNullOrEmpty(p.Text)).ToList();
            foreach (var pickListText in textsForPickList)
            {
                var rtfCleanedText = RtfCleaner.Clean(pickListText.Text);
                var truncatedText = Truncate(rtfCleanedText, 256);

                if (string.IsNullOrEmpty(truncatedText)) continue;
                
                pickListText.Text = truncatedText;
                records.Add( new GenericWamasRecord(pickListText.GetType(), pickListText));
            }

            var partnersForPickList = pickListPartners.Where(p => p.Id == pickList.Id).ToList();
            foreach (var pickListPartner in partnersForPickList)
            {
                records.Add( new GenericWamasRecord(pickListPartner.GetType(), pickListPartner));
            }
        }

        return records;
    }

    private WamasExportJobResult UpdateLogisticStates(LogisticState logisticState)
    {
        var jobResult = new WamasExportJobResult();
        
        foreach (var pickList in _pickListsToExport)
        {
            try
            {
                var pickListOrderDbModel = DatabaseConnector.GetQueryable<Document>()
                    .First(g => g.Id == int.Parse(pickList.Id));
                pickListOrderDbModel.LogisticState = logisticState;
                DatabaseConnector.Update(pickListOrderDbModel);
            }
            catch (Exception e)
            {
                jobResult.ErrorCount++;

                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasPickListConfirmError, 
                    logisticState, pickList.Id, e.Message);
                ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasPickListExportErrorTitle,
                    string.Format(ResourceIds.WamasPickListConfirmError, logisticState, pickList.Id,
                        e.Message));
            }
        }

        return jobResult;
    }

    private static string Truncate(string source, int maximumSize)
    {
        if (string.IsNullOrEmpty(source)) 
            return string.Empty;

        var transformedSource = source.Replace("\n\n", " ").Replace("\n", ", ").Trim();
        return transformedSource.Length <= maximumSize ? transformedSource : transformedSource[..maximumSize];
    }
}