using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

public class RegistrationsResult : JobResult
{
    public int AtlasRegistrationsCount { get; set; }

    public int LgavResultRegistrationsCount { get; set; }

    public int SuccessfulWriteResultCount { get; set; }
}