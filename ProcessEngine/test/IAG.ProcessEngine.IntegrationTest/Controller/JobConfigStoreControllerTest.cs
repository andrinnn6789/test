using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.ProcessEngine.IntegrationTest.Controller;

[Collection("ProcessEngineController")]
public class JobConfigStoreControllerTest
{
    private const string Uri = InfrastructureEndpoints.Process + "JobConfigStore/";
    private readonly HttpClient _client;

    public JobConfigStoreControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task GetForTemplateTest()
    {
        var response = await _client.GetAsync(Uri + "ForTemplate/"+JobInfoAttribute.GetTemplateId(typeof(HelloJob)));
        response.EnsureSuccessStatusCode();
        var config = JsonConvert.DeserializeObject<HelloConfig>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(config);
    }

    [Fact]
    public async Task CrudTest()
    {
        var config = new HelloConfig
        {
            NbOfOutputs = 42,
            Delay = 1
        };
        var responseCreated = await _client.PostAsync(Uri, 
            new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseCreated.EnsureSuccessStatusCode();

        var responseGetCreated = await _client.GetAsync(Uri + config.Id);
        responseGetCreated.EnsureSuccessStatusCode();
        var readConfigCreated = JsonConvert.DeserializeObject<HelloConfig>(await responseGetCreated.Content.ReadAsStringAsync());

        config.NbOfOutputs = 23;
        var responseUpdated = await _client.PutAsync(Uri + config.Id, 
            new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseUpdated.EnsureSuccessStatusCode();

        var responseGetUpdated = await _client.GetAsync(Uri + config.Id);
        responseGetUpdated.EnsureSuccessStatusCode();
        var readConfigUpdated = JsonConvert.DeserializeObject<HelloConfig>(await responseGetUpdated.Content.ReadAsStringAsync());

        var responsePatchPatched = await _client.PatchAsync(Uri + config.Id, new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json));
        Assert.Equal(HttpStatusCode.BadRequest, responsePatchPatched.StatusCode);

        responsePatchPatched = await _client.PatchAsync(Uri + config.Id,
            new StringContent(JsonConvert.SerializeObject(
                    new HelloConfigPatch
                    {
                        Delay = 3, 
                        Id = config.Id
                    }), 
                Encoding.UTF8, MediaTypeNames.Application.Json));
        responsePatchPatched.EnsureSuccessStatusCode();

        var responseGetPatched = await _client.GetAsync(Uri + config.Id);
        responseGetPatched.EnsureSuccessStatusCode();
        var readConfigPatched = JsonConvert.DeserializeObject<HelloConfig>(await responseGetPatched.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, responseCreated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetCreated.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, responseUpdated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetUpdated.StatusCode);
        Assert.NotNull(readConfigCreated);
        Assert.Equal(config.Id, readConfigCreated.Id);
        Assert.Equal(42, readConfigCreated.NbOfOutputs);
        Assert.NotNull(readConfigUpdated);
        Assert.Equal(config.Id, readConfigUpdated.Id);
        Assert.Equal(23, readConfigUpdated.NbOfOutputs);
        Assert.Equal(1, readConfigUpdated.Delay);
        Assert.NotNull(readConfigPatched);
        Assert.Equal(3, readConfigPatched.Delay);
        Assert.Equal(readConfigUpdated.NbOfOutputs, readConfigPatched.NbOfOutputs);
    }

    [Fact]
    public async Task CrudWithFollowUpTest()
    {
        var mainConfig = new HelloConfig() { NbOfOutputs = 42 };
        var followUpConfig = new SimplestConfig();
        mainConfig.FollowUpJobs.Add(new FollowUpJob { FollowUpJobConfigId = followUpConfig.Id });
        var responseMainCreated = await _client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(mainConfig), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseMainCreated.EnsureSuccessStatusCode();
        var responseFollowUpCreated = await _client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(followUpConfig), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseFollowUpCreated.EnsureSuccessStatusCode();

        var responseGetCreated = await _client.GetAsync(Uri + mainConfig.Id);
        responseGetCreated.EnsureSuccessStatusCode();
        var readConfigCreated = JsonConvert.DeserializeObject<HelloConfig>(await responseGetCreated.Content.ReadAsStringAsync());

        mainConfig.NbOfOutputs = 23;
        mainConfig.FollowUpJobs.Add(new FollowUpJob { FollowUpJobConfigId = followUpConfig.Id });
        var responseUpdated = await _client.PutAsync(Uri + mainConfig.Id, new StringContent(JsonConvert.SerializeObject(mainConfig), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseUpdated.EnsureSuccessStatusCode();

        var responseGetUpdated = await _client.GetAsync(Uri + mainConfig.Id);
        responseGetUpdated.EnsureSuccessStatusCode();
        var readConfigUpdated = JsonConvert.DeserializeObject<HelloConfig>(await responseGetUpdated.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, responseMainCreated.StatusCode);
        Assert.Equal(HttpStatusCode.Created, responseFollowUpCreated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetCreated.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, responseUpdated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetUpdated.StatusCode);
        Assert.NotNull(readConfigCreated);
        Assert.Equal(mainConfig.Id, readConfigCreated.Id);
        Assert.Equal(42, readConfigCreated.NbOfOutputs);
        Assert.Single(readConfigCreated.FollowUpJobs);
        Assert.NotNull(readConfigUpdated);
        Assert.Equal(mainConfig.Id, readConfigUpdated.Id);
        Assert.Equal(23, readConfigUpdated.NbOfOutputs);
        Assert.Equal(2, readConfigUpdated.FollowUpJobs.Count);
    }

    [Fact]
    public async Task GetAllTest()
    {
        var helloConfig = new HelloConfig { NbOfOutputs = 42 };
        var responseHelloCreated = await _client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(helloConfig), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseHelloCreated.EnsureSuccessStatusCode();

        var simpleConfig = new SimplestConfig { Description = "Hello World" };
        var responseSimpleCreated = await _client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(simpleConfig), Encoding.UTF8, MediaTypeNames.Application.Json));
        responseSimpleCreated.EnsureSuccessStatusCode();

        var responseGetAll = await _client.GetAsync(Uri);
        responseGetAll.EnsureSuccessStatusCode();

        var allConfigs = JsonConvert.DeserializeObject<JToken[]>(await responseGetAll.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, responseHelloCreated.StatusCode);
        Assert.Equal(HttpStatusCode.Created, responseSimpleCreated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetAll.StatusCode);
        Assert.NotNull(allConfigs);
        Assert.True(allConfigs.Length > 1);

        var helloConfigRead = Assert.Single(allConfigs, x => x.Value<string>("Id") == helloConfig.Id.ToString())?.ToObject<HelloConfig>();
        var simpleConfigRead = Assert.Single(allConfigs, x => x.Value<string>("Id") == simpleConfig.Id.ToString())?.ToObject<SimplestConfig>();
        Assert.NotNull(helloConfigRead);
        Assert.NotNull(simpleConfigRead);
        Assert.Equal(helloConfig.NbOfOutputs, helloConfigRead.NbOfOutputs);
        Assert.Equal(simpleConfig.Description, simpleConfigRead.Description);
    }

    private class HelloConfigPatch
    {
        public Guid Id { [UsedImplicitly] get; set; }
        public int Delay { [UsedImplicitly] get; set; }
    }
}