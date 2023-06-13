using System;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.Resource;

namespace IAG.InstallClient.ProcessEngineJob.SelfUpdate;

[JobInfo(JobId, JobName, true)]
public class SelfUpdateJob : JobBase<SelfUpdateJobConfig, JobParameter, JobResult>
{
    internal const string JobId = "884934B4-7110-4A31-AB5D-0E29F719E743";
    private const string JobName = ResourceIds.ResourcePrefixJob + "InstallerSelfUpdate";

    private readonly IInstallationManager _installationManager;
    private readonly ICustomerManager _customerManager;
    private readonly IReleaseManager _releaseManager;

    public SelfUpdateJob(ICustomerManager customerManager, IReleaseManager releaseManager,
        IInstallationManager installationManager)
    {
        _customerManager = customerManager;
        _releaseManager = releaseManager;
        _installationManager = installationManager;
    }

    protected override void ExecuteJob()
    {
        try
        {
            DoSelfUpdateAsync().Wait(JobCancellationToken);
        }
        catch (Exception ex)
        {
            AddMessage(MessageTypeEnum.Error, ResourceIds.SelfUpdateFailed, LocalizableException.GetExceptionMessage(ex));
            Result.Result = JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }

    private async Task DoSelfUpdateAsync()
    {
        var customerInfo = _customerManager.GetCurrentCustomerInformationAsync().Result;
        if (customerInfo == null)
        {
            throw new LocalizableException(ResourceIds.SelfUpdateNoCustomerInfo);
        }

        ReportProgress(0.05);
        AddMessage(MessageTypeEnum.Information, ResourceIds.SelfUpdateGetUpdaterProduct);
        var products = (await _releaseManager.GetProductsAsync(customerInfo.Id)).ToList();

        ReportProgress(0.10);
        var updaterProduct = products.FirstOrDefault(p => p.ProductType == ProductType.Updater);
        if (updaterProduct == null)
        {
            throw new LocalizableException(ResourceIds.SelfUpdateNoUpdaterProduct);
        }

        ReportProgress(0.15);
        AddMessage(MessageTypeEnum.Information, ResourceIds.SelfUpdateGetUpdaterRelease);
        var newestUpdaterRelease = await GetNewestReleaseAsync(customerInfo.Id, updaterProduct.Id);
        if (newestUpdaterRelease == null)
        {
            throw new LocalizableException(ResourceIds.SelfUpdateNoUpdaterRelease);
        }

        if (newestUpdaterRelease.ReleaseVersion == _installationManager.CurrentSelfVersion)
        {
            AddMessage(MessageTypeEnum.Information, ResourceIds.SelfUpdateNoNewVersion);
            return;
        }

        ReportProgress(0.20);
        AddMessage(MessageTypeEnum.Information, ResourceIds.SelfUpdateGetConfig);
        var configTemplateProduct = products.FirstOrDefault(p =>
            p.ProductType == ProductType.ConfigTemplate && p.DependsOnProductId == updaterProduct.Id);
        var newestConfigTemplate = await GetNewestReleaseAsync(customerInfo.Id, configTemplateProduct?.Id);

        var setup = new InstallationSetup()
        {
            CustomerId = customerInfo.Id,
            ProductId = newestUpdaterRelease.ProductId,
            ReleaseId = newestUpdaterRelease.Id,
            ConfigurationProductId = newestConfigTemplate?.ProductId,
            ConfigurationReleaseId = newestConfigTemplate?.Id
        };

        var messageLogger = new SubMessageLogger(this, 0.2, 1.0);
        await _installationManager.DoSelfUpdate(setup, messageLogger);
        AddMessage(MessageTypeEnum.Information, ResourceIds.SelfUpdateDone, newestUpdaterRelease.ReleaseVersion, newestConfigTemplate?.ReleaseVersion);
    }

    private async Task<ReleaseInfo> GetNewestReleaseAsync(Guid customerId, Guid? productId)
    {
        if (productId == null)
            return null;

        var releases = await _releaseManager.GetReleasesAsync(customerId, productId.Value);
        return releases.OrderByDescending(r => r.ReleaseVersion).FirstOrDefault();
    }
}