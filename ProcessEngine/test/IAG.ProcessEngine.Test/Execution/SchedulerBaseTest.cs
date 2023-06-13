using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class SchedulerBaseTest
{
    [Fact]
    public async Task FaultyGetNextExecutionTimesTest()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockLogger = new MockILogger<FaultyImplementation>();

        var faultyScheduler = new FaultyImplementation(mockServiceProvider.Object, mockLogger) 
            {FailInGetNextExecutionTimes = true};

        faultyScheduler.Start();
        await Task.Delay(2000);
        var started = faultyScheduler.IsRunning;
        faultyScheduler.Stop();

        Assert.True(started);
        Assert.False(faultyScheduler.IsRunning);
        Assert.Equal(2, mockLogger.LogEntries.Count);
        Assert.Contains(FaultyImplementation.ExceptionMessage, mockLogger.LogEntries.First());
    }

    [Fact]
    public async Task FaultyOnBeforEnqueueJobTest()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockLogger = new MockILogger<FaultyImplementation>();

        var faultyScheduler = new FaultyImplementation(mockServiceProvider.Object, mockLogger);
        faultyScheduler.OnBeforeEnqueueJob += FaultyScheduler_OnBeforeEnqueueJob;

        faultyScheduler.Start();
        await Task.Delay(2000);
        var started = faultyScheduler.IsRunning;
        faultyScheduler.Stop();

        Assert.True(started);
        Assert.False(faultyScheduler.IsRunning);
        Assert.Equal(2, mockLogger.LogEntries.Count);
        Assert.Contains(FaultyImplementation.ExceptionMessage, mockLogger.LogEntries.First());
    }

    internal static IServiceScope CreateServiceProvider(IJobService jobService, IJobStore jobStore, IJobConfigStore jobConfigStore)
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        mockServiceScope.Setup(m => m.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(m => m.CreateScope()).Returns(mockServiceScope.Object);

        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobService)))).Returns(jobService);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IServiceScopeFactory)))).Returns(mockServiceScopeFactory.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobStore)))).Returns(jobStore);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobConfigStore)))).Returns(jobConfigStore);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IUserContext)))).Returns(new ExplicitUserContext("testUser", null));

        return mockServiceScope.Object;
    }

    private void FaultyScheduler_OnBeforeEnqueueJob(IJobInstance jobInstance)
    {
        throw new System.Exception(FaultyImplementation.ExceptionMessage);
    }

    private class FaultyImplementation : SchedulerBase
    {
        public FaultyImplementation(IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
        {
        }

        public static string ExceptionMessage => "I will never cooperate!";

        public bool FailInGetNextExecutionTimes { get; set; }

        protected override SortedDictionary<DateTime, IJobInstance> GetNextExecutionTimes(DateTime lastCheckTimeUtc, int secondsAhead)
        {
            if (FailInGetNextExecutionTimes)
                throw new System.Exception(ExceptionMessage);

            return new SortedDictionary<DateTime, IJobInstance> { { DateTime.UtcNow, null } };
        }
    }
}