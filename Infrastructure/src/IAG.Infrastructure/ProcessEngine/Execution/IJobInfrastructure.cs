using System;

using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.JobData;
using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.Infrastructure.ProcessEngine.Execution;

public interface IJobInfrastructure : IJobHeartbeatObserver
{
    void HeartbeatAndReportProgress(double progress);

    void PersistState();

    void AddMessage(MessageStructure message);

    void EnqueueFollowUpJob(Guid jobConfigId, IJobParameter parameter = null);

    T GetJobData<T>() where T : IJobData, new();

    void SetJobData<T>(T data) where T : IJobData;

    void CancelJob();
}