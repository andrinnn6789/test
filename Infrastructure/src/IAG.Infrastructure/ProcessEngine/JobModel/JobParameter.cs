using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

[Export(typeof(IJobParameter))]
[ExcludeFromCodeCoverage]
public class JobParameter : IJobParameter
{
    public DateTime TimeToRunUtc { get; set; }
}