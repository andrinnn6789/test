using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.Smith.BossExport.ProcessEngine;

[ExcludeFromCodeCoverage]
public class BossExportSyncState: IJobData
{
    public Guid Id { get; set; }

    public List<int> ExportedIds { get; set; } = new();
}