using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Cron;
using IAG.Infrastructure.ProcessEngine.JobModel;

using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.ProcessEngine.Configuration;

[ExcludeFromCodeCoverage]
[Export(typeof(IJobConfig))]
public abstract class JobConfig<TJob> : IJobConfig
{
    private string _cronExpression;

    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string SerializedData { get; set; }

    public bool Active { get; set; } = true;

    public bool AllowConcurrentInstances { get; set; }

    public TimeSpan HeartbeatTimeout { get; set; } = new(0, 0, 1, 0);

    public bool LogActivity { get; set; } = true;

    public LogLevel LogLevel { get; set; }

    public TimeSpan[] RetryIntervals { get; set; }

    public string CronExpression
    {
        get => _cronExpression;
        set
        {
            if (!string.IsNullOrEmpty(value))
                CronParser.Parse(value);
            _cronExpression = value;
        }
    }

    public List<FollowUpJob> FollowUpJobs { get; set; }

    protected JobConfig()
    {
        var jobInfo = JobInfoAttribute.GetJobInfo(typeof(TJob));
        Id = Guid.NewGuid();
        TemplateId = jobInfo.TemplateId;
        Name = jobInfo.Name;
        FollowUpJobs = new List<FollowUpJob>();
    }
}