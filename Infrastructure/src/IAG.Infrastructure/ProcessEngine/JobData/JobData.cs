using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.ProcessEngine.JobData;

[ExcludeFromCodeCoverage]
[Export(typeof(IJobData))]
public class JobData : IJobData
{
    /// <summary>
    /// Equals the job-template, is managed by the system
    /// </summary>
    public Guid Id { get; set; }
}