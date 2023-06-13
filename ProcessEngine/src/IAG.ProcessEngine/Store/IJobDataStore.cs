using System;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.ProcessEngine.Store;

public interface IJobDataStore
{
    void Set<T>(IJobData jobData) where T : IJobData;

    T Get<T>(Guid id) where T : IJobData, new();

    void Remove(Guid id);

    void SetRaw(Guid id, string data);

    string GetRaw(Guid id);

}