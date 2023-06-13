using System;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.Execution;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

public interface IJob
{
    Guid TemplateId { get; }

    string Name { get; }

    IJobConfig Config { get; set; }

    IJobParameter Parameter { get; set; }

    IJobResult Result { get; }

    bool Execute(IJobInfrastructure infrastructure);
}