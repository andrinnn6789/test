using System;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Resource;

namespace IAG.InstallClient.ProcessEngineJob.Transfer;

[JobInfo(JobId, JobName, true)]
public class TransferJob : JobBase<TransferJobConfig, TransferJobParameter, JobResult>
{
    internal const string JobId = "9FE48CE3-202F-4364-9A11-E5B3EC5B5924";
    internal const string JobName = ResourceIds.ResourcePrefixJob + "Transfer";

    private readonly IInstallationManager _installationManager;
    private readonly IInventoryManager _inventoryManager;
    private readonly IServiceManager _serviceManager;

    public TransferJob(IInstallationManager installationManager, IInventoryManager inventoryManager,
        IServiceManager serviceManager)
    {
        _installationManager = installationManager;
        _inventoryManager = inventoryManager;
        _serviceManager = serviceManager;
    }

    protected override void ExecuteJob()
    {
        try
        {
            DoTransferInstallationAsync().Wait(JobCancellationToken);
        }
        catch (Exception ex)
        {
            AddMessage(MessageTypeEnum.Error, ResourceIds.TransferFailed, ex.Message);
            Result.Result = JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }

    private async Task DoTransferInstallationAsync()
    {
        _installationManager.TransferInstance(Parameter.SourceInstanceName, Parameter.TargetInstanceName, this);
        AddMessage(MessageTypeEnum.Information, ResourceIds.TransferRegisterInstallation);
        await _inventoryManager.RegisterInstallationAsync(Parameter.CustomerId,
            new InstallationRegistration
            {
                InstanceName = Parameter.TargetInstanceName,
                ReleaseVersion = Parameter.TargetVersion,
                Description = $"Transferred from '{Parameter.SourceInstanceName}'"
            });

        if (!string.IsNullOrEmpty(Parameter.ServiceToStart))
        {
            AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationStartService);
            _serviceManager.StartService(Parameter.ServiceToStart);
        }
    }
}