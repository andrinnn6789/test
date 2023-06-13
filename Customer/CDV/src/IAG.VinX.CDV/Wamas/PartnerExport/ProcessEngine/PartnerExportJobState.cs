using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.CDV.Wamas.PartnerExport.ProcessEngine;

public class PartnerExportJobState : IJobData
{
    [ExcludeFromCodeCoverage] public Guid Id { get; set; }

    public DateTime LastSync { get; set; } = DateTime.MinValue;
}