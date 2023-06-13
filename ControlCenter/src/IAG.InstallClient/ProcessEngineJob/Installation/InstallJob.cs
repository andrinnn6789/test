    using System;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Resource;

    namespace IAG.InstallClient.ProcessEngineJob.Installation; 

    [JobInfo(JobId, JobName, true)]
    public class InstallJob : JobBase<InstallJobConfig, InstallJobParameter, InstallJobResult>
    {
        internal const string JobId = "81135995-553C-4A55-86E3-5EC4D11CE154";
        internal const string JobName = ResourceIds.ResourcePrefixJob + "Installation";

        private readonly IInstallationManager _installationManager;
        private readonly IServiceManager _serviceManager;

        public InstallJob(IInstallationManager installationManager, IServiceManager serviceManager)
        {
            _installationManager = installationManager;
            _serviceManager = serviceManager;
        }

        protected override void ExecuteJob()
        {
            try
            {
                Result.InstanceName = _installationManager.CreateOrUpdateInstallationAsync(Parameter.Setup, this).Result;
                if (!string.IsNullOrEmpty(Parameter.ServiceToStart))
                {
                    AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationStartService);
                    _serviceManager.StartService(Parameter.ServiceToStart);
                }
            }
            catch (Exception ex)
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.InstallationFailed, ex.Message);
                Result.Result = JobResultEnum.Failed;
            }

            base.ExecuteJob();
        }
    }