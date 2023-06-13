using System;

using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.InstallClient.ProcessEngineJob.Transfer;

public class TransferJobParameter: JobParameter
{
    public Guid CustomerId { get; set; }
    public string SourceInstanceName { get; set; }
    public string TargetInstanceName { get; set; }
    public string TargetVersion { get; set; }
    public string ServiceToStart { get; set; }
}