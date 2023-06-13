using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;

public class SyncState: IJobData
{
    [ExcludeFromCodeCoverage]
    public Guid Id { get; set; }

    public DateTime LastSync { get; set; } = DateTime.MinValue;
}