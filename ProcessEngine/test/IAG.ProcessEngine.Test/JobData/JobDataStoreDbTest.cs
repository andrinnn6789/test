using System;

using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.ProcessEngine.DataLayer.State.Context;
using IAG.ProcessEngine.Store;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.ProcessEngine.Test.JobData;

public class JobDataStoreDbTest
{
    private readonly IJobDataStore _store;

    public JobDataStoreDbTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProcessEngineStateDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        var context = new ProcessEngineStateDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
        _store = new JobDataStoreDb(context);
    }
        
    [Fact]
    public void GetFromEmptyStoreTest()
    {
        var jobId = Guid.NewGuid();
        var data = _store.Get<TestJobData>(jobId);

        Assert.NotNull(data);
        Assert.Equal(jobId, data.Id);
    }

    [Fact]
    public void GetRawFromEmptyStoreTest()
    {
        var data = _store.GetRaw(Guid.NewGuid());

        Assert.Null(data);
    }

    [Fact]
    public void SetAndGetTest()
    {
        var dataIn = new TestJobData()
        {
            Id = Guid.NewGuid(),
            TestNumber = 42,
            TestString = "Hello World",
            TestGuid = Guid.NewGuid(),
        };

        _store.Set<TestJobData>(dataIn);

        var dataOut = _store.Get<TestJobData>(dataIn.Id);

        Assert.NotNull(dataOut);
        Assert.Equal(dataIn.TestNumber, dataOut.TestNumber);
        Assert.Equal(dataIn.TestString, dataOut.TestString);
        Assert.Equal(dataIn.TestGuid, dataOut.TestGuid);
    }


    [Fact]
    public void SetAndGetRawTest()
    {
        var jobId = Guid.NewGuid();
        var dataIn = "Top Secret Test Data!";

        _store.SetRaw(jobId, dataIn);

        var dataOut = _store.GetRaw(jobId);

        Assert.NotNull(dataOut);
        Assert.Equal(dataIn, dataOut);
    }

    [Fact]
    public void SetAndMultipleRemoveTest()
    {
        var dataIn = new TestJobData()
        {
            Id = Guid.NewGuid(),
            TestNumber = 42,
            TestString = "Hello World",
            TestGuid = Guid.NewGuid(),
        };

        _store.Set<TestJobData>(dataIn);
        _store.Remove(dataIn.Id);
        _store.Remove(dataIn.Id);   // Should not throw an exception

        var dataOut = _store.Get<TestJobData>(dataIn.Id);

        Assert.NotNull(dataOut);
        Assert.NotEqual(dataIn.TestNumber, dataOut.TestNumber);
        Assert.NotEqual(dataIn.TestString, dataOut.TestString);
        Assert.NotEqual(dataIn.TestGuid, dataOut.TestGuid);
    }

    private class TestJobData : Infrastructure.ProcessEngine.JobData.JobData
    {
        public int TestNumber { get; set; }
        public string TestString { get; set; }
        public Guid TestGuid { get; set; }
    }
}