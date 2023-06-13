using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.ProcessEngine.InternalJob.Cleanup;
using IAG.ProcessEngine.Store;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.InternalJob.CleanUp;

public class CleanupJobTest
{
    [Fact]
    public void ExecuteTest()
    {
        int archiveDays = 0, errorDays = 0;

        var jobStore = new Mock<IJobStore>();
        jobStore.Setup(m => m.DeleteOldJobs(It.IsAny<int>(), It.IsAny<int>())).Callback<int, int>(
            (ad, ed) =>
            {
                archiveDays = ad;
                errorDays = ed;
            });

        var messages = new List<MessageStructure>();
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.AddMessage(It.IsAny<MessageStructure>())).Callback<MessageStructure>(msg => messages.Add(msg));

        var job = new CleanupJob(jobStore.Object) { Config = new CleanupJobConfig { ArchiveDays = 31, ErrorDays = 42 } };

        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.Empty(messages.Where(m => m.Type == MessageTypeEnum.Error));
        Assert.Equal(job.Config.ArchiveDays, archiveDays);
        Assert.Equal(job.Config.ErrorDays, errorDays);
    }
}