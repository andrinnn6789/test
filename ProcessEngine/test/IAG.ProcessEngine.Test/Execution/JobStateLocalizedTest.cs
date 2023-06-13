using System;
using System.Collections.Generic;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class JobStateLocalizedTest
{
    private static string _translationPrefix = "Translation for ";

    [Fact]
    public void Test()
    {
        var state = new JobState
        {
            TemplateId = Guid.NewGuid(),
            DateCreation = DateTime.UtcNow,
            DateDue = DateTime.UtcNow,
            MetadataId = Guid.NewGuid(),
            Owner = "Me",
            ContextTenant = Guid.NewGuid(),
            RetryCounter = 3,
            DateRunStart = DateTime.UtcNow,
            Progress = 0.2,
            ExecutionState = JobExecutionStateEnum.Running
        };

        var msg = new MessageStructure(MessageTypeEnum.Information, "Test", 42);
        state.Messages.Add(msg);
        var mockLocalizer = new Mock<IMessageLocalizer>();
        mockLocalizer.Setup(m => m.Localize(It.IsAny<List<MessageStructure>>()))
            .Returns(() => new List<MessageStructureLocalized>
            {
                new(msg.Type, "TypeName", _translationPrefix + msg.ResourceId, msg.Timestamp)
            });

        var stateLocalized = new JobStateLocalized(state, mockLocalizer.Object);

        Assert.NotEqual(Guid.Empty, stateLocalized.Id);
        Assert.NotEmpty(stateLocalized.LocalizedMessages);
        Assert.Equal(_translationPrefix + state.Messages[0].ResourceId, stateLocalized.LocalizedMessages[0].Text);
        Assert.Equal(state.TemplateId, stateLocalized.TemplateId);
        Assert.Equal(state.JobConfigId, stateLocalized.JobConfigId);
        Assert.Equal(state.ParentJob, stateLocalized.ParentJob);
        Assert.Equal(state.ChildJobs, stateLocalized.ChildJobs);
        Assert.Equal(state.DateCreation, stateLocalized.DateCreation);
        Assert.Equal(state.DateDue, stateLocalized.DateDue);
        Assert.Equal(state.IsBlocking, stateLocalized.IsBlocking);
        Assert.Equal(state.MetadataId, stateLocalized.MetadataId);
        Assert.Equal(state.Owner, stateLocalized.Owner);
        Assert.Equal(state.ContextTenant, stateLocalized.Tenant);
        Assert.Equal(state.RetryCounter, stateLocalized.RetryCounter);
        Assert.Equal(state.Parameter, stateLocalized.Parameter);
        Assert.Equal(state.DateRunStart, stateLocalized.DateRunStart);
        Assert.Equal(state.Progress, stateLocalized.Progress);
        Assert.Equal(state.DateRunEnd, stateLocalized.DateRunEnd);
        Assert.Equal(state.ExecutionState, stateLocalized.ExecutionState);
        Assert.Equal(state.Result, stateLocalized.Result);
    }
}