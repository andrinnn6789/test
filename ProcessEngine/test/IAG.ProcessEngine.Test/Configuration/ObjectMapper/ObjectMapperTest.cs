using System;
using System.Collections.Generic;

using FluentAssertions;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.ProcessEngine.DataLayer.State.ObjectMapper;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;

using Xunit;

namespace IAG.ProcessEngine.Test.Configuration.ObjectMapper;

public class ObjectMapperTest
{
    private readonly Random _rand;

    public ObjectMapperTest()
    {
        _rand = new Random();
    }

    [Fact]
    public void MapToDbWithoutExceptionTest()
    {
        var jobState = CreateRandomJobState();

        var result = new JobStateToDbMapper().NewDestination(jobState);
        Assert.NotNull(result);
    }

    [Fact]
    public void MapToDbWithoutExceptionWithoutParamAndResultTest()
    {
        var jobState = new JobState
        {
            TemplateId = Guid.NewGuid(),
            DateCreation = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsBlocking = _rand.Next(0, 1) == 1,
            Messages = CreateRandomMessages(),
            Owner = "TestOwner",
            ContextTenant = Guid.NewGuid(),
            Progress = _rand.NextDouble(),
            RetryCounter = _rand.Next(5),
            ExecutionState = JobExecutionStateEnum.Running
        };

        var result = new JobStateToDbMapper().NewDestination(jobState);
        Assert.NotNull(result);
    }

    [Fact]
    public void MapBackAndForthTest()
    {
        var jobState = CreateRandomJobState();

        var jobStateDb = new JobStateToDbMapper().NewDestination(jobState);
        Assert.NotNull(jobStateDb);

        var jobStateMappedBack = new JobStateFromDbMapper().NewDestination(jobStateDb);
        Assert.NotNull(jobStateMappedBack);

        jobStateMappedBack.Should().BeEquivalentTo(jobState);
    }

    [Fact]
    public void MapBackAndForthWithParentAndChildTest()
    {
        var jobStateParent = CreateRandomJobState();
        var jobState = CreateRandomJobState(jobStateParent);
        var jobStateChild = CreateRandomJobState();
        jobState.ChildJobs.Add(jobStateChild);

        var jobStateDb = new JobStateToDbMapper().NewDestination(jobState);
        Assert.NotNull(jobStateDb);
        Assert.NotNull(jobStateDb.ParentJobId);
        Assert.Equal(1, jobStateDb.ChildJobIds.Count);

        // recreate parents and children
        var jobStateParentDb = new JobStateToDbMapper().NewDestination(jobStateParent);
        var jobStateChildDb = new JobStateToDbMapper().NewDestination(jobStateChild);
        jobStateDb.ParentJob = jobStateParentDb;
        jobStateDb.ChildJobs.Add(jobStateChildDb);            
        var jobStateMappedBack = new JobStateFromDbMapper().NewDestination(jobStateDb);
        Assert.NotNull(jobStateMappedBack);

        jobState.Should().BeEquivalentTo(jobStateMappedBack);
    }

    private IJobState CreateRandomJobState(IJobState parentJob = null)
    {
        return new JobState
        {
            TemplateId = Guid.NewGuid(),
            DateCreation = DateTime.UtcNow,
            Id = Guid.NewGuid(), 
            IsBlocking = _rand.Next(0, 1) == 1,
            Messages = CreateRandomMessages(),
            Owner = "TestOwner",
            ContextTenant = Guid.NewGuid(),
            Progress = _rand.NextDouble(),
            RetryCounter = _rand.Next(5),
            ExecutionState = JobExecutionStateEnum.Running,
            ChildJobs = new List<IJobState>(),
            ParentJob = parentJob,
            Parameter = new HelloParameter
            {
                GreetingsFrom = "Hallo",
                IgnoreJobCancel = true,
                ThrowException = true
            },
            Result = new HelloResult
            {
                NbExecutions = _rand.Next(10)
            }
        };
    }

    private static List<MessageStructure> CreateRandomMessages()
    {
        var result = new List<MessageStructure>
        {
            new(MessageTypeEnum.Information, "Test 1"),
            new(MessageTypeEnum.Description,
                "Test with params",
                new LocalizableParameter(
                    "abs",
                    1)),
            new(MessageTypeEnum.Description,
                "Test with empty params",
                new LocalizableParameter(null, null))
        };
        return result;
    }
}