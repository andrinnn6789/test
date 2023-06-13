using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.Enum;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

[ExcludeFromCodeCoverage]
public class JobResult : IJobResult
{
    public JobResultEnum Result { get; set; }

    public int ErrorCount { get; set; }
}