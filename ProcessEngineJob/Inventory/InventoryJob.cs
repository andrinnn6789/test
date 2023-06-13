using System;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Influx;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Resource;

using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace IAG.InstallClient.ProcessEngineJob.Inventory;

[JobInfo(JobId, JobName, true)]
public class InventoryJob : JobBase<InventoryJobConfig, JobParameter, JobResult>
{
    private const string JobId = "A38973D6-F9E5-44E4-801B-387D696EBC05";
    private const string JobName = ResourceIds.ResourcePrefixJob + "Inventory";

    private readonly IInstallationManager _installationManager;
    private readonly ICustomerManager _customerManager;
    private readonly IInfluxClient _influxClient;
    const string InfluxBucket = "installerInventory";

    public InventoryJob(IInstallationManager installationManager,
        ICustomerManager customerManager,
        IInfluxClient influxClient)
    {
        _installationManager = installationManager;
        _customerManager = customerManager;
        _influxClient = influxClient;
    }

    protected override void ExecuteJob()
    {
        try
        {
            DoInventoryAsync().Wait(JobCancellationToken);
        }
        catch (Exception ex)
        {
            AddMessage(MessageTypeEnum.Error, ResourceIds.InventoryFailed, LocalizableException.GetExceptionMessage(ex));
            Result.Result = JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }

    private async Task DoInventoryAsync()
    {
        CustomerInfo customerInfo = await _customerManager.GetCurrentCustomerInformationAsync();
        if (customerInfo == null)
        {
            throw new LocalizableException(ResourceIds.InventoryNoCustomerInfo);
        }

        var installationDataPoint = PointData.Measurement("installerInstallation")
            .Field("customerId", customerInfo.Id.ToString())
            .Tag("customerName", customerInfo.CustomerName)
            .Tag("version", _installationManager.CurrentSelfVersion)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ms);
        try
        {
            await _influxClient.SendDataPointAsync(InfluxBucket, installationDataPoint);
        }
        catch (Exception ex)
        {
            AddMessage(MessageTypeEnum.Error, ResourceIds.InventoryAddInstallationFailed, LocalizableException.GetExceptionMessage(ex));
        }
    }
}