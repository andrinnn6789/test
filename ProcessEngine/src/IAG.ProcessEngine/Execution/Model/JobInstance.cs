using System;

using IAG.Infrastructure.ProcessEngine.JobModel;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.ProcessEngine.Execution.Model;

public class JobInstance : IJobInstance
{
    private readonly IServiceScope _serviceScope;

    public JobInstance(IServiceScope serviceScope)
    {
        _serviceScope = serviceScope;
    }

    public Guid Id => State.Id;
        
    public IJobState State { get; set; }
        
    public IJob Job { get; set; }

    public IServiceProvider ServiceProvider => _serviceScope?.ServiceProvider;

    public void Dispose()
    {
        _serviceScope?.Dispose();
    }
}