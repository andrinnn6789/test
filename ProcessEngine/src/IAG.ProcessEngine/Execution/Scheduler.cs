using System;
using System.Collections.Generic;

using IAG.Infrastructure.Cron;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Execution;

public class Scheduler : SchedulerBase, IScheduler
{
    private static readonly string SchedulerUserName = "job-scheduler";

    public Scheduler(IServiceProvider serviceProvider, ILogger<Scheduler> logger) : base(serviceProvider, logger)
    {
    }

    protected override SortedDictionary<DateTime, IJobInstance> GetNextExecutionTimes(DateTime lastCheckTimeUtc, int secondsAhead)
    {

        var dict = new SortedDictionary<DateTime, IJobInstance>();
        using var scope = ServiceProvider.CreateScope();
        var jobConfigStore = scope.ServiceProvider.GetRequiredService<IJobConfigStore>();

        foreach (var jobConfig in jobConfigStore.GetAll())
        {
            if (string.IsNullOrEmpty(jobConfig.CronExpression) || !jobConfig.Active)
            {
                continue;
            }

            foreach (var runTime in GetNextCronExecutionTimes(jobConfig.CronExpression, lastCheckTimeUtc, secondsAhead))
            {
                var timeToRun = runTime;
                while (dict.ContainsKey(timeToRun))
                {
                    timeToRun = timeToRun.AddTicks(1);
                }

                var jobScope = ServiceProvider.CreateScope();
                jobScope.ServiceProvider.GetRequiredService<IUserContext>().SetExplicitUser(SchedulerUserName, null);
                var jobInstance = jobScope.ServiceProvider.GetRequiredService<IJobService>().CreateJobInstance(jobConfig.Id);

                dict.Add(timeToRun, jobInstance);
            }
        }

        return dict;
    }

    private IEnumerable<DateTime> GetNextCronExecutionTimes(string cronExpression, DateTime lastCheckTimeUtc, int secondsAhead)
    {
        var checkUntilUtc = lastCheckTimeUtc.AddSeconds(secondsAhead);
        var nextUtc = CronParser.Parse(cronExpression, lastCheckTimeUtc.ToLocalTime(), true)?.ToUniversalTime();

        while (nextUtc.HasValue && nextUtc < checkUntilUtc)
        {
            yield return nextUtc.Value;
            nextUtc = CronParser.Parse(cronExpression, nextUtc.Value.ToLocalTime(), true)?.ToUniversalTime();
        }
    }
}