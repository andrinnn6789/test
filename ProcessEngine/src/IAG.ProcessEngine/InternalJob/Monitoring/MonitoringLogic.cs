using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Influx;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

using Microsoft.Extensions.Localization;

namespace IAG.ProcessEngine.InternalJob.Monitoring;

public class MonitoringLogic
{
    private const string InfluxBucket = "processEngine";
    private const string InfluxTableJobStates = "peJobStates";
    private const string InfluxTableJobMessages = "peJobMessages";
    private const string InfluxIsAliveMessage = "isAlive";

    private readonly MonitoringJobConfig _jobConfig;
    private readonly IJobHeartbeatObserver _jobHeartbeatObserver;
    private readonly IJobStore _jobStore;
    private readonly IJobCatalogue _jobCatalogue;
    private readonly IInfluxClient _influxDbClient;
    private readonly MessageLocalizer _messageLocalizer;

    public MonitoringLogic(IJobHeartbeatObserver jobHeartbeatObserver, MonitoringJobConfig jobConfig, IJobStore jobStore,
        IJobCatalogue jobCatalogue, IInfluxClient influxClient, IStringLocalizer stringLocalizer)
    {
        _jobHeartbeatObserver = jobHeartbeatObserver;
        _jobStore = jobStore;
        _jobConfig = jobConfig;
        _jobCatalogue = jobCatalogue;
        _influxDbClient = influxClient;
        _messageLocalizer = new MessageLocalizer(stringLocalizer);
    }

    public int UpdateStatus(DateTime lastStatusUpdate, Guid excludeTemplateId)
    {
        var sentCount = 0;
        var jobStates = _jobStore.GetJobs()
            .OrderByDescending(j => j.DateRunEnd)
            .Where(j => j.DateRunEnd >= lastStatusUpdate && j.TemplateId != excludeTemplateId).Take(100);
        foreach (var jobState in jobStates)
        {
            if (jobState.Result == null || (int)jobState.Result.Result < (int)_jobConfig.MinExportState)
            {
                continue;
            }

            var timestamp = new DateTimeOffset(Debugger.IsAttached 
                ? DateTime.Now 
                : jobState.DateRunEnd ?? DateTime.Now);

            var jobMetadata = _jobCatalogue.Catalogue.FirstOrDefault(j => j.TemplateId == jobState.TemplateId);
            var dataPoint = BuildPoint(InfluxTableJobStates, jobState, jobMetadata?.PluginName)
                .Field("executionState", (int)jobState.ExecutionState)
                .Field("result", (int)jobState.Result.Result)
                .Field("retryCounter", jobState.RetryCounter)
                .Field("dateRunStart",jobState.DateRunStart.ToString())
                .Field("dateRunEnd", jobState.DateRunEnd.ToString())
                    
                .Timestamp(timestamp, WritePrecision.S);

            _influxDbClient.SendDataPointAsync(InfluxBucket, dataPoint).Wait();

            var offset = 0;
            foreach (var message in jobState.Messages)
            {
                offset++;
                var localizedMessage = _messageLocalizer.Localize(message);
                var msgText = localizedMessage.Text.Replace("\\", "\\\\").Replace("\"", "\\\"");
                msgText = Regex.Replace(msgText, @"\r\n?|\n", "   ");

                dataPoint = BuildPoint(InfluxTableJobMessages, jobState, jobMetadata?.PluginName)
                    .Tag("typeId", ((int)localizedMessage.Type).ToString())
                    .Tag("typeName", localizedMessage.Type.ToString())
                    .Field("message", msgText)

                    .Timestamp(timestamp.AddMilliseconds(offset), WritePrecision.Ms);

                _influxDbClient.SendDataPointAsync(InfluxBucket, dataPoint).Wait();
            }

            sentCount++;
            _jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();
        }

        SendKeepAlive();

        return sentCount;
    }

    private void SendKeepAlive()
    {
        var dataPoint = new DatapointBuilder().BuildPoint(InfluxIsAliveMessage, _jobConfig.CustomerName)
            .Field("isAlive", 1)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ms);

        _influxDbClient.SendDataPointAsync(InfluxBucket, dataPoint).Wait();
    }

    private PointData BuildPoint(string measurement, IJobState jobState, string pluginName)
    {
        return new DatapointBuilder().BuildPoint(measurement, _jobConfig.CustomerName)
            .Tag("jobConfigId", jobState.JobConfigId.ToString())
            .Tag("jobConfigName", _jobConfig.Name)
            .Tag("jobTemplateId", jobState.TemplateId.ToString())
            .Tag("jobTemplateName", pluginName)
            .Tag("host", Environment.MachineName);
    }
}