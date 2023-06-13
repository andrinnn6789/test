using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Schüwo.SV.Dto;

namespace IAG.VinX.Schüwo.SV.ProcessEngine;

public class SvBaseJobResult : JobResult
{
    public ResultCounts ResultCounts { get; set; } = new();
}