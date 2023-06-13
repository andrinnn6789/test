using System;

using IAG.Infrastructure.Influx;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Resource;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.Localization;

namespace IAG.ProcessEngine.InternalJob.Monitoring;

[JobInfo("8FB2EBB9-0B02-49B8-B1AD-9C2D796B14EB", JobName, true)]
public class MonitoringJob : JobBase<MonitoringJobConfig, JobParameter, MonitoringResult>
{
    internal const string JobName = ResourceIds.ResourcePrefixJob + "Monitoring";

    private readonly IJobStore _jobStore;
    private readonly IJobCatalogue _jobCatalogue;
    private readonly IStringLocalizer<MonitoringJob> _stringLocalizer;
    private readonly IInfluxClient _influxClient;

    public MonitoringJob(IJobStore jobStore, IJobCatalogue jobCatalogue, IStringLocalizer<MonitoringJob> stringLocalizer, IInfluxClient influxClient)
    {
        _jobStore = jobStore;
        _jobCatalogue = jobCatalogue;
        _stringLocalizer = stringLocalizer;
        _influxClient = influxClient;
    }

    protected override void ExecuteJob()
    {
        var logic = new MonitoringLogic(Infrastructure, Config, _jobStore, _jobCatalogue, _influxClient, _stringLocalizer);
        var jobData = Infrastructure.GetJobData<MonitoringJobData>();

        var newStatusUpdate = DateTime.UtcNow;
        try
        {
            Result.SendCount = logic.UpdateStatus(jobData.LastStatusUpdate ?? DateTime.UtcNow, TemplateId);
            Result.Result = JobResultEnum.Success;
            jobData.LastStatusUpdate = newStatusUpdate;
            Infrastructure.SetJobData(jobData);
        }
        catch (System.Exception ex)
        {
            AddMessage(ex);
            Result.Result = JobResultEnum.Failed;
        }

        base.ExecuteJob();
    }
}