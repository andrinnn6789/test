using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.ProcessEngine;

public class WorklogSyncState: IJobData
{
    [ExcludeFromCodeCoverage]
    public Guid Id { get; set; }

    public DateTime LastSync { get; set; } = DateTime.Now;
}