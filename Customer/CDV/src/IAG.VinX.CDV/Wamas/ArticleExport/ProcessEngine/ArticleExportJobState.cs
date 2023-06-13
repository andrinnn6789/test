using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.JobData;

namespace IAG.VinX.CDV.Wamas.ArticleExport.ProcessEngine;

public class ArticleExportJobState : IJobData
{
    [ExcludeFromCodeCoverage] public Guid Id { get; set; }

    public DateTime LastSync { get; set; } = DateTime.MinValue;
}