using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.ProcessEngine.DataLayer.State.Context;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.ProcessEngine.Test.Configuration;

public class JobStoreDbTest
{
    private readonly IJobStore _store;

    private static readonly Guid JobId1 = Guid.NewGuid();
    private static readonly Guid JobId2 = Guid.NewGuid();
    private static readonly Guid JobId3 = Guid.NewGuid();
    private static readonly Guid JobId4 = Guid.NewGuid();
    private static readonly Guid JobId5 = Guid.NewGuid();

    public JobStoreDbTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProcessEngineStateDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        var context = new ProcessEngineStateDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
        context.JobStateEntries.AddRange(
            new DataLayer.State.Model.JobState
            {
                // job 1, done
                Id = JobId1,
                DateCreation = DateTime.UtcNow.AddHours(-3),
                DateRunEnd = DateTime.Today.AddHours(-2),
                ExecutionState = JobExecutionStateEnum.Success,
                Progress = 100
            },
            new DataLayer.State.Model.JobState
            {
                // job 2, child of job 1, scheduled after job 3
                Id = JobId2,
                DateCreation = DateTime.UtcNow,
                DateDue = DateTime.UtcNow,
                ParentJobId = JobId1,
                ExecutionState = JobExecutionStateEnum.Scheduled
            },
            new DataLayer.State.Model.JobState
            {
                // job 3, child of job 2, scheduled before job 2
                Id = JobId3,
                DateCreation = DateTime.UtcNow,
                DateDue = DateTime.Today,
                ParentJobId = JobId2,
                ExecutionState = JobExecutionStateEnum.Scheduled
            },
            new DataLayer.State.Model.JobState
            {
                // job 4, failed
                Id = JobId4,
                DateCreation = DateTime.UtcNow,
                DateDue = DateTime.Today.AddDays(-5),
                ExecutionState = JobExecutionStateEnum.Failed
            },
            new DataLayer.State.Model.JobState
            {
                // job 5, old, done
                Id = JobId5,
                DateCreation = DateTime.UtcNow.AddDays(-2),
                DateRunEnd = DateTime.Today.AddDays(-2),
                ExecutionState = JobExecutionStateEnum.Success
            });
        context.SaveChanges();
        _store = new JobStoreDb(context);
    }

    [Fact]
    public void DeleteJobTest()
    {
        _store.Delete(JobId4);
        Assert.Throws<NotFoundException>(() => _store.Get(JobId4));
        Assert.Throws<NotFoundException>(() => _store.Delete(JobId4));
    }

    [Fact]
    public void DeleteOldJobsTest()
    {
        _store.DeleteOldJobs(1, 10);
        var job = _store.GetJobs();
        var jobCount = _store.GetJobCount();
        Assert.Equal(4, job.Count()); // archivDays = 1, errorDays = 5 -> delete only 1 old job
        Assert.Equal(jobCount, job.Count());
    }

    [Fact]
    public void DeleteScheduledJobsTest()
    {
        _store.DeleteScheduledJobs();
        var job = _store.GetJobs();
        var jobCount = _store.GetJobCount();
        Assert.Equal(3, job.Count());
        Assert.Equal(jobCount, job.Count());
    }

    [Fact]
    public void GetJobByIdTest()
    {
        var job = _store.Get(JobId1);
        Assert.Equal(JobId1, job.Id);
    }

    [Fact]
    public void GetJobs()
    {
        var job = _store.GetJobs();
        var jobCount = _store.GetJobCount();
        Assert.Equal(5, job.Count());
        Assert.Equal(jobCount, job.Count());

        job = _store.GetJobs().Skip(2).Take(2);
        Assert.Equal(2, job.Count());

        job = _store.GetJobs().Where(j => j.ExecutionState == JobExecutionStateEnum.Scheduled || j.ExecutionState == JobExecutionStateEnum.Success);
        jobCount = _store.GetJobCount(new[] {JobExecutionStateEnum.Scheduled, JobExecutionStateEnum.Success});
        Assert.Equal(4, job.Count());
        Assert.Equal(jobCount, job.Count());

        job = _store.GetJobs().Where(j => j.ExecutionState == JobExecutionStateEnum.Failed);
        jobCount = _store.GetJobCount(new[] {JobExecutionStateEnum.Failed});
        Assert.Single(job);
        Assert.Equal(jobCount, job.Count());
    }

    [Fact]
    public void InsertJobTest()
    {
        var jobId = Guid.NewGuid();
        var parentJob = _store.Get(JobId4);
        Assert.Empty(parentJob.ChildJobs);
        var job = new JobState
        {
            Id = jobId,
            ParentJob = parentJob,
            MetadataId = jobId,
            Messages = new List<MessageStructure>
            {
                new(MessageTypeEnum.Information, "hello"),
                new(MessageTypeEnum.Description, "hello {0}", "world")
            }
        };
        _store.Insert(job);
        var jobRead = _store.Get(jobId);
        Assert.Equal(JobId4, jobRead.ParentJob.Id);
        parentJob = _store.Get(JobId4);
        Assert.Single(parentJob.ChildJobs);
        Assert.Equal(jobId, parentJob.ChildJobs[0].Id);
        Assert.Throws<DuplicateKeyException>(() => _store.Insert(job));
    }

    [Fact]
    public void UpdateJobTest()
    {
        var job = new JobState
        {
            Id = JobId1,
            Progress = 50
        };
        _store.Update(job);
        var jobRead = _store.Get(JobId1);
        Assert.Equal(50, jobRead.Progress);
        job.Id = Guid.NewGuid();
        Assert.Throws<NotFoundException>(() => _store.Update(job));
    }

    [Fact]
    public void UpsertJobTest()
    {
        var jobId = Guid.NewGuid();
        var job = new JobState
        {
            Id = jobId,
            Progress = 50
        };
        _store.Upsert(job);
        var jobRead = _store.Get(jobId);
        Assert.Equal(50, jobRead.Progress);
        job.Progress = 80;
        _store.Upsert(job);
        jobRead = _store.Get(jobId);
        Assert.Equal(80, jobRead.Progress);
    }
}