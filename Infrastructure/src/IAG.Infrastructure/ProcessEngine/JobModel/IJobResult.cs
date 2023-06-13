using IAG.Infrastructure.ProcessEngine.Enum;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

public interface IJobResult
{
    JobResultEnum Result { get; set; }

    public int ErrorCount { get; set; }
}