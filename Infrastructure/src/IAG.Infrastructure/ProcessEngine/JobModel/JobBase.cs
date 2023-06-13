using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;

namespace IAG.Infrastructure.ProcessEngine.JobModel;

[ExcludeFromCodeCoverage]  // is in project ProcessEngine tested
public abstract class JobBase<TConfig, TParam, TResult> : IJob, IMessageLogger
    where TConfig : class, IJobConfig, new()
    where TParam : class, IJobParameter, new()
    where TResult : class, IJobResult, new()
{
    private TConfig _config;
    private TParam _parameter;
    private TResult _result;

    protected JobBase()
    {
        Result = new TResult();
        Parameter = new TParam();

        var jobInfoAttribute = JobInfoAttribute.GetJobInfo(GetType());
        TemplateId = jobInfoAttribute.TemplateId;
        Name = jobInfoAttribute.Name;

    }

    public Guid TemplateId { get; }

    public string Name { get; }

    public TConfig Config
    {
        get => _config ??= new TConfig();
        set => _config = value;
    }

    IJobConfig IJob.Config
    {
        get => Config;
        set => Config = value as TConfig;
    }

    public TParam Parameter
    {
        get => _parameter ??= new TParam();
        set => _parameter = value;
    }

    IJobParameter IJob.Parameter
    {
        get => Parameter;
        set => Parameter = value as TParam;
    }

    public TResult Result
    {
        get => _result ??= new TResult();
        private set => _result = value;
    }

    IJobResult IJob.Result => Result;

    public CancellationToken JobCancellationToken => Infrastructure.JobCancellationToken;

    protected IJobInfrastructure Infrastructure { get; private set; }

    public bool Execute(IJobInfrastructure infrastructure)
    {
        Infrastructure = infrastructure;
        try
        {
            ExecuteJob();

            if (Result.Result == JobResultEnum.NoResult)
            {
                Result.Result = Result.ErrorCount == 0 ? JobResultEnum.Success : JobResultEnum.PartialSuccess;
            }

            return true;
        }
        catch (OperationCanceledException) 
        {
            throw;  // caught in JobExecuter!
        }
        catch (System.Exception ex)
        {
            this.LogException(Config.Name, ex);
            // just in case the job was cancelled and the OperationCanceledException is in an AggregateException
            Infrastructure.JobCancellationToken.ThrowIfCancellationRequested();
            Result.Result = JobResultEnum.Failed;
            Result.ErrorCount++;
            return false;
        }
    }

    public void AddMessage(MessageTypeEnum type, string resourceId, params object[] args)
    {
        Infrastructure.Heartbeat();
        Infrastructure.AddMessage(new MessageStructure(type, resourceId, args));
    }

    public void AddMessage(MessageStructure message)
    {
        Infrastructure.Heartbeat();
        Infrastructure.AddMessage(message);
    }

    public void AddMessage(System.Exception e)
    {
        Infrastructure.Heartbeat();
        Infrastructure.AddMessage(new MessageStructure(e));
    }

    public void ReportProgress(double progress)
        => HeartbeatAndReportProgress(progress);

    protected void Heartbeat()
    {
        Infrastructure.Heartbeat();
    }

    protected void HeartbeatAndCheckCancellation()
    {
        Infrastructure.HeartbeatAndCheckJobCancellation();
    }

    protected void HeartbeatAndReportProgress(double progress)
    {
        Infrastructure.HeartbeatAndReportProgress(progress);
    }

    protected virtual void ExecuteJob()
    {
    }
}