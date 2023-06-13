using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Store.Model;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ProcessEngine.IntegrationTest.Controller;

[Collection("ProcessEngineController")]
public class JobCatalogueControllerTest
{
    private const string Uri = InfrastructureEndpoints.Process + "JobCatalogue/";
    private readonly HttpClient _client;

    public JobCatalogueControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public void GetAllTest()
    {
        var response = _client.GetAsync(Uri).Result;
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var catalogue = JsonConvert.DeserializeObject<List<JobMetadata>>(response.Content.ReadAsStringAsync().Result);
        Assert.NotNull(catalogue);
        Assert.NotEmpty(catalogue);
        Assert.True(catalogue[0].TemplateId != Guid.Empty, "template-id of job ok");
    }

    [Fact]
    public void GetByTemplateIdTest()
    {
        var response = _client.GetAsync(Uri + JobInfoAttribute.GetTemplateId(typeof(HelloJob))).Result;
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jobMetadata = JsonConvert.DeserializeObject<JobMetadata>(response.Content.ReadAsStringAsync().Result);
        Assert.NotNull(jobMetadata);
    }

    [Fact]
    public void GetByTemplateIdNotFoundTest()
    {
        var response = _client.GetAsync(Uri + Guid.NewGuid()).Result;
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}