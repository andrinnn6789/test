using IAG.ProcessEngine.InternalJob.Cleanup;
using IAG.ProcessEngine.InternalJob.Monitoring;

namespace IAG.ProcessEngine.Resource;

public static class ResourceIds
{
    public const string ResourcePrefix = "ProcessEngine.";
        
    internal const string JobExecuterJobCanceled = ResourcePrefix + "JobExecuter.JobCanceled";
    internal const string JobExecuterSingletonJobAlreadyRunning = ResourcePrefix + "JobExecuter.SingletonJobAlreadyRunning";
    internal const string JobExecuterUnhandledException = ResourcePrefix + "JobExecuter.UnhandledException";
    internal const string JobDataStoreNotForConcurrentJobs = ResourcePrefix + "JobDataStore.NotForConcurrentJobs";

    internal const string ConditionParseExceptionUnknownOperator = ResourcePrefix + "ConditionParseException.UnknownOperator";
    internal const string ConditionParseExceptionInvalidOperatorUsage = ResourcePrefix + "ConditionParseException.InvalidOperatorUsage";
    internal const string ConditionParseExceptionInvalidExpression = ResourcePrefix + "ConditionParseException.InvalidExpression";
    internal const string ConditionParseExceptionEmptyExpression = ResourcePrefix + "ConditionParseException.EmptyExpression";
    internal const string ConditionParseExceptionMissingClosingQuote = ResourcePrefix + "ConditionParseException.MissingClosingQuote";
    internal const string ConditionParseExceptionParenthesisMismatch = ResourcePrefix + "ConditionParseException.ParenthesisMismatch";
    internal const string ConditionParseExceptionUnknownCondition = ResourcePrefix + "ConditionParseException.UnknownCondition";

    internal const string MonitoringSendInfluxPointFailed = ResourcePrefix + "Monitoring.SendInfluxPointFailed";

    // jobs
    public const string ResourcePrefixJob = ResourcePrefix + "Job.";
    internal const string MonitoringJobName = MonitoringJob.JobName;
    internal const string CleanupJobName = CleanupJob.JobName;

}