using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IAG.Infrastructure.ProcessEngine.Configuration;

public interface IJobConfig
{
    Guid Id { get; set; }

    Guid TemplateId { get; set; }

    string Name { get; set; }

    string Description { get; set; }

    [JsonIgnore]
    string SerializedData { get; set; }

    bool Active { get; set; }

    bool AllowConcurrentInstances { get; set; }

    TimeSpan HeartbeatTimeout { get; set; }

    bool LogActivity { get; set; }

    LogLevel LogLevel { get; set; }

    TimeSpan[] RetryIntervals { get; set; }

    string CronExpression { get; set; }

    List<FollowUpJob> FollowUpJobs { get; set; }
}