using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.ProcessEngine.Provider;
using IAG.ProcessEngine.Startup;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

using FollowUpJob = IAG.Infrastructure.ProcessEngine.Configuration.FollowUpJob;

namespace IAG.ProcessEngine.Test.Provider;

public class JobConfigProviderTest
{
    [Fact]
    public void GetAll_ShouldReturnJobConfigs()
    {
        // Arrange
        var jobConfigs = new List<FakeJobConfig>()
        {
            new(),
            new(),
            new(),
        };
        
        var fakeJobConfigStore = new Mock<IJobConfigStore>();
        fakeJobConfigStore.Setup(jobConfigStore => jobConfigStore.GetAll()).Returns(jobConfigs);
        
        var fakeServiceProvider = new Mock<IServiceProvider>();
        fakeServiceProvider.Setup(sp => sp.GetService(typeof(IJobConfigStore)))
            .Returns(fakeJobConfigStore.Object);

        ServiceProviderHolder.ServiceProvider = fakeServiceProvider.Object;
        var testee = new JobConfigProvider();
        
        // Act
        var result = testee.GetAll();
        
        // Assert
        Assert.Equal(jobConfigs.Count, result.Count());
    }
}

public class FakeJobConfig : IJobConfig
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SerializedData { get; set; }
    public bool Active { get; set; }
    public bool AllowConcurrentInstances { get; set; }
    public TimeSpan HeartbeatTimeout { get; set; }
    public bool LogActivity { get; set; }
    public LogLevel LogLevel { get; set; }
    public TimeSpan[] RetryIntervals { get; set; }
    public string CronExpression { get; set; }
    public List<FollowUpJob> FollowUpJobs { get; set; }
}