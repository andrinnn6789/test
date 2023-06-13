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

public class MonitoringJobTest
{
    private readonly List<IJobState> _jobStates;
    private readonly MonitoringJobData _jobData;
    private readonly MonitoringJob _job;
    private readonly List<MessageStructure> _messages;
    private readonly Mock<IJobInfrastructure> _jobInfrastructureMock;
    private readonly Mock<IInfluxClient> _influxMock;

    public MonitoringJobTest()
    {
        _jobStates = new List<IJobState>();
        _jobData = new MonitoringJobData();

        var mockJobStore = new Mock<IJobStore>();
        mockJobStore.Setup(m => m.GetJobs()).Returns(() => _jobStates.AsQueryable());

        var jobCatalogue = new Mock<IJobCatalogue>();
        jobCatalogue.Setup(m => m.Catalogue).Returns(new List<IJobMetadata>
        {
            new JobMetadata {TemplateId = JobInfoAttribute.GetTemplateId(typeof(MonitoringJob))}
        });

        _messages = new List<MessageStructure>();
        _jobInfrastructureMock = new Mock<IJobInfrastructure>();
        _influxMock = new Mock<IInfluxClient>();
        _jobInfrastructureMock.Setup(m => m.AddMessage(It.IsAny<MessageStructure>())).Callback<MessageStructure>(msg => _messages.Add(msg));
        _jobInfrastructureMock.Setup(m => m.GetJobData<MonitoringJobData>()).Returns(_jobData);

        _job = new MonitoringJob(mockJobStore.Object, jobCatalogue.Object, new MockLocalizer<MonitoringJob>(), _influxMock.Object)
        {
            Config = new MonitoringJobConfig
            {
                CustomerName = "Testing",
                MinExportState = JobResultEnum.PartialSuccess
            }
        };
    }

    [Fact]
    public void ExecuteWithoutRelevantStatesTest()
    {
        var executed = _job.Execute(_jobInfrastructureMock.Object);
        Assert.True(executed);

        _jobData.LastStatusUpdate = DateTime.UtcNow;
        _jobStates.Add(new JobState {DateRunEnd = DateTime.UtcNow.AddHours(-1)});
        _jobStates.Add(new JobState {DateRunEnd = null});
        executed = _job.Execute(_jobInfrastructureMock.Object); 
        Assert.True(executed);
        Assert.Empty(_messages);
        Assert.True(_job.Result.SendCount == 0);
    }

    [Fact]
    public void ExecuteFailTest()
    {
        _influxMock
            .Setup(m => m.SendDataPointAsync(It.IsAny<string>(), It.IsAny<PointData>()))
            .ThrowsAsync(new System.Exception());
        _jobData.LastStatusUpdate = DateTime.UtcNow.AddHours(-1);
        _jobStates.Add(new JobState
        {
            DateRunEnd = DateTime.UtcNow.AddSeconds(-2), ExecutionState = JobExecutionStateEnum.Success, Messages =
                new List<MessageStructure>
                {
                    new(MessageTypeEnum.Debug, "Strange.Message", DateTime.MinValue)
                },
            Result = new JobResult
            {
                Result = JobResultEnum.Failed
            }
        });
        var executed = _job.Execute(_jobInfrastructureMock.Object);
        Assert.True(executed);
        Assert.NotEmpty(_messages);
    }
}