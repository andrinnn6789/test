using System.Collections.Generic;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.ProcessEngine.Startup;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.ProcessEngine.Provider;

public class JobConfigProvider : IJobConfigProvider
{
    public IEnumerable<IJobConfig> GetAll()
    {
        var jobConfigStore = ServiceProviderHolder.ServiceProvider.GetRequiredService<IJobConfigStore>();
        return jobConfigStore.GetAll();
    }
}