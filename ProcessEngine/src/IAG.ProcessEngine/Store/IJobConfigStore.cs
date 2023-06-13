using System;
using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.ProcessEngine.Store;

public interface IJobConfigStore
{
    void Insert(IJobConfig config);

    IEnumerable<IJobConfig> GetAll();

    IEnumerable<IJobConfig> GetAllUnprocessed();

    IJobConfig Read(Guid id);

    IJobConfig ReadUnprocessed(Guid id);

    void Update(IJobConfig config);

    void Delete(IJobConfig config);

    IEnumerable<IFollowUpJob> GetFollowUpJobs(Guid id);

    IJobConfig GetOrCreateJobConfig(string jobConfigName);

    void EnsureConfig(Guid templateId, Type configType);
}