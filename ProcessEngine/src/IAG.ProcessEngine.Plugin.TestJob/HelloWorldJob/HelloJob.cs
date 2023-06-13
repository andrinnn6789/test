using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;

[UsedImplicitly]
[JobInfo("145CF263-C074-4C89-A3BF-B7111498FCCF", JobName)]
public class HelloJob : JobBase<HelloConfig, HelloParameter, HelloResult>
{
    public const string JobName = "Test.HelloJob";

    protected override void ExecuteJob()
    {
        AddMessage(new MessageStructure(MessageTypeEnum.Information, "HelloJob started..."));
        for (var i = 0; i < Config.NbOfOutputs; i++)
        {
            if (!Parameter.IgnoreJobCancel)
            {
                HeartbeatAndCheckCancellation();
                Task.Delay(Config.Delay).Wait(JobCancellationToken);
            }
            else
            {
                Task.Delay(Config.Delay).Wait();
            }
            AddMessage(MessageTypeEnum.Information, "Hello world no {0} from {1}", i, Parameter.GreetingsFrom);
            Result.NbExecutions++;
            HeartbeatAndReportProgress((i + 1.0) / Config.NbOfOutputs);
        }

        if (Parameter.ThrowException)
        {
            var ex = new System.Exception("Ex is thrown!");
            AddMessage(MessageTypeEnum.Error, "HelloJob will fail...");
            AddMessage(ex);
            throw ex;
        }

        if (Parameter.ThrowLocalizableException)
        {
            var ex = new LocalizableException("{0} is thrown!", "LocalizableException");
            AddMessage(MessageTypeEnum.Error, "HelloJob will fail...");
            AddMessage(ex);
            throw ex;
        }

        Result.Result = JobResultEnum.Success;

        AddMessage(MessageTypeEnum.Information, "HelloJob finished.");

        if (Parameter.WithFollowUp)
        {
            Infrastructure.EnqueueFollowUpJob(JobInfoAttribute.GetTemplateId(typeof(SimplestJob.SimplestJob)));
            Infrastructure.EnqueueFollowUpJob(
                TemplateId,
                new HelloParameter
                {
                    GreetingsFrom = "Follow-Up",
                    TimeToRunUtc = Parameter.TimeToRunUtc
                }
            );
        }

        base.ExecuteJob();
    }
}