using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.IAG.ControlCenter.Config;

public class SyncState: IJobData
{
    [ExcludeFromCodeCoverage]
    public Guid Id { get; set; }

    public DateTime LastUpload { get; set; } = DateTime.MinValue;

    public int SyncCounter { get; set; }
}