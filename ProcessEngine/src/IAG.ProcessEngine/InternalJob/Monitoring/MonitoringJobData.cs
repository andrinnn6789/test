using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.InternalJob.Monitoring;

[UsedImplicitly]
public class MonitoringJobData : IJobData
{
    [ExcludeFromCodeCoverage]
    public Guid Id { get; set; }

    [UsedImplicitly]
    public DateTime? LastStatusUpdate { get; set; }
}