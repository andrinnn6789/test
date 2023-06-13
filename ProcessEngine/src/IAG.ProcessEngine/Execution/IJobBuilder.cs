using System;

using IAG.ProcessEngine.Execution.Model;

namespace IAG.ProcessEngine.Execution;

public interface IJobBuilder
{
    IJobInstance BuildInstance(Guid jobConfigId, IJobState jobStateParent);

    IJobInstance ReBuildInstance(IJobState jobState);
}