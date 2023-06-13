using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Xunit;

namespace IAG.ProcessEngine.IntegrationTest.Controller;

[Collection("ProcessEngineController")]
public class JobServiceControllerTest
{
    private const string UriJobService = InfrastructureEndpoints.ProcessJobService;    
    private const string UriJobConfigStore = InfrastructureEndpoints.Process + "JobConfigStore/";
    private readonly TestServerEnvironment _testSeverEnvironment;
    private readonly HttpClient _client;
    private static bool _defaultConfigAdded;

    public JobServiceControllerTest(TestServerEnvironment testEnvironment)
    {
        _testSeverEnvironment = testEnvironment;
        _client = _testSeverEnvironment.NewClient();
        AddDefaultConfigurations(_client).Wait();
        _client.PostAsync(UriJobService + "Start", new StringContent(string.Empty)).Result.EnsureSuccessStatusCode();
    }

    [Fact]
    public async void ExecuteHelloWithFollowUp()
    {
        var helloParam = new HelloParameter
        {
            GreetingsFrom = "Hello-Job",
            IgnoreJobCancel = false,
            WithFollowUp = true,
            ThrowException = false,
            ThrowLocalizableException = false
        };
        var responseExecute = await _client.PostAsync(
            UriJobService + "Execute?jobConfigId=" + JobInfoAttribute.GetTemplateId(typeof(HelloJob)),
            new StringContent(JsonConvert.SerializeObject(helloParam), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseExecute.EnsureSuccessStatusCode();

        var jobState = JsonConvert.DeserializeObject<JobState>(
            responseExecute.Content.ReadAsStringAsync().Result,
            new JobParameterConverter(),
            new JobResultConverter());
        var response = await _client.GetAsync(UriJobService + "GetJobInstanceState?jobInstanceId=" + jobState?.Id);
        response.EnsureSuccessStatusCode();
        jobState = JsonConvert.DeserializeObject<JobState>(
            response.Content.ReadAsStringAsync().Result,
            new JobParameterConverter(),
            new JobResultConverter(),
            new JobStateConverter());

        Assert.NotNull(jobState);
        Assert.True(jobState.ChildJobs.Count == 2, "2 child jobs found");
        Assert.Equal(HttpStatusCode.OK, responseExecute.StatusCode);
        Assert.Equal(JobExecutionStateEnum.Success, jobState.ExecutionState);
        Assert.Equal(UserContext.AnonymousUserName, jobState.Owner);

        response = await _client.GetAsync(UriJobService + "GetJobInstanceState?jobInstanceId=" + jobState.ChildJobs[0].Id);
        response.EnsureSuccessStatusCode();
        var jobStateChild = JsonConvert.DeserializeObject<JobState>(
            response.Content.ReadAsStringAsync().Result,
            new JobParameterConverter(),
            new JobResultConverter(),
            new JobStateConverter());

        Assert.NotNull(jobStateChild);
        Assert.NotNull(jobStateChild.ParentJob);
        Assert.Equal(jobState.Id, jobStateChild.ParentJob.Id);
        Assert.Equal(UserContext.AnonymousUserName, jobStateChild.Owner);
    }

    [Fact]
    public async void ExecuteHelloWithFollowUpTimed()
    {
        var helloParam = new HelloParameter
        {
            GreetingsFrom = "Hello-Job",
            WithFollowUp = true,
            TimeToRunUtc = DateTime.UtcNow.AddMilliseconds(500)
        };
        var responseExecute = await _client.PostAsync(
            UriJobService + "Execute?jobConfigId=" + JobInfoAttribute.GetTemplateId(typeof(HelloJob)),
            new StringContent(JsonConvert.SerializeObject(helloParam), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseExecute.EnsureSuccessStatusCode();

        var jobState = JsonConvert.DeserializeObject<JobState>(
            responseExecute.Content.ReadAsStringAsync().Result,
            new JobParameterConverter(),
            new JobResultConverter());
        var response = await _client.GetAsync(UriJobService + "GetJobInstanceState?jobInstanceId=" + jobState?.Id);
        response.EnsureSuccessStatusCode();
        jobState = JsonConvert.DeserializeObject<JobState>(
            response.Content.ReadAsStringAsync().Result,
            new JobParameterConverter(),
            new JobResultConverter(),
            new JobStateConverter());

        Assert.NotNull(jobState);
        Assert.Equal(HttpStatusCode.OK, responseExecute.StatusCode);
        Assert.Equal(JobExecutionStateEnum.Success, jobState.ExecutionState);
        Assert.True(jobState.ChildJobs.Count == 2, "2 child job found");
        IJobState jobStateTimed = null;
        IJobState jobStateImmediate = null;
        foreach (var jobStateChild in jobState.ChildJobs)
        {
            if (jobStateChild.JobConfigId == JobInfoAttribute.GetTemplateId(typeof(SimplestJob)))
                jobStateImmediate = jobStateChild;
            if (jobStateChild.JobConfigId == JobInfoAttribute.GetTemplateId(typeof(HelloJob)))
                jobStateTimed = jobStateChild;
        }
        Assert.NotNull(jobStateImmediate);
        Assert.NotNull(jobStateTimed);
        Assert.Equal(JobExecutionStateEnum.Success, jobStateImmediate.ExecutionState);
        Assert.Equal(JobExecutionStateEnum.New, jobStateTimed.ExecutionState);
            
        var serviceLifetimes = _testSeverEnvironment.GetServices().GetServices<IServiceLifetime>().ToArray();
        foreach (var serviceLifetime in serviceLifetimes)
        {
            serviceLifetime.OnStart();
        }

        try
        {
            await Task.Delay(1000);

            response = await _client.GetAsync(UriJobService + "GetJobInstanceState?jobInstanceId=" + jobStateTimed.Id);
            response.EnsureSuccessStatusCode();
            jobState = JsonConvert.DeserializeObject<JobState>(
                response.Content.ReadAsStringAsync().Result,
                new JobParameterConverter(),
                new JobResultConverter(),
                new JobStateConverter());
            Assert.NotNull(jobState);
            Assert.Equal(JobExecutionStateEnum.Success, jobState.ExecutionState);
        }
        finally
        {
            foreach (var serviceLifetime in serviceLifetimes)
            {
                serviceLifetime.OnStop();
            }
        }
    }

    [Fact]
    public async void ExecuteJob()
    {
        var helloParam = new HelloParameter
        {
            GreetingsFrom = "Hello-Job",
            IgnoreJobCancel = false,
            ThrowLocalizableException = false
        };
        var responseExecute = await _client.PostAsync(
            UriJobService + "ExecuteJob?jobConfigName=" + HelloJob.JobName,
            new StringContent(JsonConvert.SerializeObject(helloParam), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseExecute.EnsureSuccessStatusCode();
    }

    private async Task AddDefaultConfigurations(HttpClient client)
    {
        if (_defaultConfigAdded)
            return;
        await AddDefaultConfiguration(new HelloConfig(), client);
        await AddDefaultConfiguration(new SimplestConfig(), client);
        _defaultConfigAdded = true;
    }

    private async Task AddDefaultConfiguration(IJobConfig jobConfig, HttpClient client)
    {
        jobConfig.Id = jobConfig.TemplateId;
        var response = await client.PostAsync(
            UriJobConfigStore, new StringContent(JsonConvert.SerializeObject(jobConfig), Encoding.UTF8, MediaTypeNames.Application.Json));
        response.EnsureSuccessStatusCode();
    }
}

public class JobParameterConverter : CustomCreationConverter<IJobParameter>
{
    public override IJobParameter Create(Type objectType)
    {
        return new HelloParameter();
    }
}

public class JobResultConverter : CustomCreationConverter<IJobResult>
{
    public override IJobResult Create(Type objectType)
    {
        return new HelloResult();
    }
}

public class JobStateConverter : CustomCreationConverter<IJobState>
{
    public override IJobState Create(Type objectType)
    {
        return new JobState();
    }
}