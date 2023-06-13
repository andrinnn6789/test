using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.Extensions.DependencyInjection;

using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Execution;

public class Monitor : SchedulerBase, IMonitor
{
    public Monitor(IServiceProvider serviceProvider, ILogger<Monitor> logger) : base(serviceProvider, logger)
    {
    }

    protected override SortedDictionary<DateTime, IJobInstance> GetNextExecutionTimes(DateTime lastCheckTimeUtc, int secondsAhead)
    {
        SortedDictionary<DateTime, IJobInstance> dict = new SortedDictionary<DateTime, IJobInstance>();

        var checkTillUtc = lastCheckTimeUtc.AddSeconds(secondsAhead);
        using var scope = ServiceProvider.CreateScope();
        var jobStore = scope.ServiceProvider.GetRequiredService<IJobStore>();
        var jobs = jobStore.GetJobs().Where(j => j.ExecutionState == JobExecutionStateEnum.New && j.DateDue <= checkTillUtc);

        foreach (var jobState in jobs)
        {
            var jobScope = ServiceProvider.CreateScope();
            jobScope.ServiceProvider.GetRequiredService<IUserContext>().SetExplicitUser(jobState.Owner, jobState.ContextTenant);
            var jobInstance = jobScope.ServiceProvider.GetRequiredService<IJobService>().CreateJobInstance(jobState);
            var timeToRun = jobState.DateDue;
            while (dict.ContainsKey(timeToRun))
            {
                timeToRun = timeToRun.AddTicks(1);
            }

            dict.Add(timeToRun, jobInstance);
        }

        return dict;
    }
}