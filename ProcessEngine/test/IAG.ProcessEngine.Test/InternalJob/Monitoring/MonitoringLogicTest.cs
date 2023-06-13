using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Influx;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.InternalJob.Monitoring;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using InfluxDB.Client.Writes;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.InternalJob.Monitoring;

public class MonitoringLogicTest
{
    private readonly List<IJobState> _jobStates;
    private readonly List<PointData> _dataPoints;
    private readonly MonitoringLogic _monitoringLogic;
    private readonly IJobState _oldJobState;
    private readonly IJobState _runningJobState;
    private readonly IJobState _jobStateWithoutMessages;
    private readonly IJobState _jobStateWithMessages;

    public MonitoringLogicTest()
    {
        _jobStates = new List<IJobState>();

        var jobHeartbeatObserver = new Mock<IJobHeartbeatObserver>();

        var mockJobStore = new Mock<IJobStore>();
        mockJobStore.Setup(m => m.GetJobs()).Returns(() => _jobStates.AsQueryable());

        _dataPoints = new List<PointData>();
        var influxClient = new Mock<IInfluxClient>();
        influxClient.Setup(m => m.SendDataPointAsync(It.IsAny<string>(), It.IsAny<PointData>()))
            .Callback<string, PointData>((_, p) => _dataPoints.Add(p));

        var jobCatalogue = new Mock<IJobCatalogue>();
        jobCatalogue.Setup(m => m.Catalogue).Returns(new List<IJobMetadata>
        {
            new JobMetadata {TemplateId = JobInfoAttribute.GetTemplateId(typeof(MonitoringJob))}
        });

        var jobConfig = new MonitoringJobConfig
        {
            MinExportState = JobResultEnum.PartialSuccess
        };

        _monitoringLogic = new MonitoringLogic(jobHeartbeatObserver.Object, jobConfig, mockJobStore.Object, jobCatalogue.Object, influxClient.Object,
            new MockLocalizer<MonitoringLogicTest>());

        _oldJobState = new JobState {DateRunEnd = DateTime.UtcNow.AddDays(-2)};
        _runningJobState = new JobState {DateRunEnd = null};
        _jobStateWithoutMessages = new JobState
        {
            DateRunEnd = DateTime.UtcNow.AddSeconds(-1),
            ExecutionState = JobExecutionStateEnum.Success,
            Result = new JobResult
            {
                Result = JobResultEnum.Success
            }
        };
        var messageTimestamp = DateTime.UtcNow.AddSeconds(-2);
        _jobStateWithMessages = new JobState
        {
            DateRunEnd = DateTime.UtcNow.AddSeconds(-2), ExecutionState = JobExecutionStateEnum.Success, Messages = new List<MessageStructure>
            {
                new(MessageTypeEnum.Debug, "Strange.Message", DateTime.MinValue),
                new(MessageTypeEnum.Information, "Just.A.Test", messageTimestamp),
                new(MessageTypeEnum.Information, "Just.Another.Test", messageTimestamp),
                new(MessageTypeEnum.Information, "And.A.Last.One", messageTimestamp)
            },
            Result = new JobResult
            {
                Result = JobResultEnum.Failed
            }
        };
    }

    [Fact]
    public void EmptyTest()
    {
        _monitoringLogic.UpdateStatus(DateTime.UtcNow.AddDays(-10), Guid.NewGuid());

        Assert.Single(_dataPoints);
    }

    [Fact]
    public void FirstRunTest()
    {
        _jobStates.Add(_oldJobState);
        _jobStates.Add(_runningJobState);
        _jobStates.Add(_jobStateWithoutMessages);
        _jobStates.Add(new JobState
        {
            DateRunEnd = DateTime.UtcNow.AddSeconds(-1),
            ExecutionState = JobExecutionStateEnum.Success,
            Result = new JobResult
            {
                Result = JobResultEnum.PartialSuccess
            }
        });

        _monitoringLogic.UpdateStatus(DateTime.Now.AddDays(-1), Guid.NewGuid());

        Assert.Equal(2, _dataPoints.Count);
    }

    [Fact]
    public void OnlyIrrelevantStatesTest()
    {
        _jobStates.Add(_oldJobState);
        _jobStates.Add(_runningJobState);

        _monitoringLogic.UpdateStatus(DateTime.UtcNow.AddSeconds(-10), Guid.NewGuid());

        Assert.Single(_dataPoints);
    }

    [Fact]
    public void StateWithMessagesTest()
    {
        _jobStates.Add(_jobStateWithMessages);

        _monitoringLogic.UpdateStatus(DateTime.UtcNow.AddSeconds(-10), Guid.NewGuid());

        Assert.Equal(2 + _jobStateWithMessages.Messages.Count, _dataPoints.Count);
    }
}