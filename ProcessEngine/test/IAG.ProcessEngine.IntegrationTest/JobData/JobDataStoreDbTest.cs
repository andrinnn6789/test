using IAG.Infrastructure.TestHelper.Startup;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IAG.ProcessEngine.IntegrationTest.JobData;

[Collection("ProcessEngineController")]
public class JobDataStoreDbTest
{
    private readonly TestServerEnvironment _testEnvironment;

    public JobDataStoreDbTest(TestServerEnvironment testEnvironment)
    {
        _testEnvironment = testEnvironment;
    }

    [Fact]
    public void MultipleSetAndGetTest()
    {
        var serviceScope = _testEnvironment.GetServices().CreateScope();
        var jobInstance = new JobInstance(serviceScope);
        jobInstance.Job = new SimplestJob();
        var jobService = serviceScope.ServiceProvider.GetRequiredService<IJobService>();
        var jobInfrastructure = new JobExecutionInfrastructure(jobInstance, jobService);

        var dataIn = new TestJobData()
        {
            Id = Guid.NewGuid()
        };
        var dataOut = new List<TestJobData>();
        var runs = 100;
        for (int i = 0; i < runs; i++)
        {
            dataIn.TestNumber = i;
            dataIn.TestString = $"TestRun {i}";

            jobInfrastructure.SetJobData(dataIn);
            dataOut.Add(jobInfrastructure.GetJobData<TestJobData>());
        }

        Assert.Equal(runs, dataOut.Count);
        for (int i = 0; i < runs; i++)
        {
            Assert.Equal(i, dataOut[i].TestNumber);
            Assert.Equal($"TestRun {i}", dataOut[i].TestString);
        }
    }

    [Fact]
    public void ParallelSetAndGetTest()
    {
        MultipleSetAndGetTest(); // Ensure single configuration...
        Parallel.Invoke(Enumerable.Repeat(MultipleSetAndGetTest, 50).ToArray());
    }

    private class TestJobData : Infrastructure.ProcessEngine.JobData.JobData
    {
        public int TestNumber { get; set; }
        public string TestString { get; set; }
    }
}