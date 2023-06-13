using System;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

public interface IJobParameter
{
    DateTime TimeToRunUtc { get; set; }
}