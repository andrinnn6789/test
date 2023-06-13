using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Zweifel.S1M.Dto.RequestModels;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Zweifel.IntegrationTest.S1M.CoreServer;

public class ControllersTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = "/api/Swiss1Mobile/";
    private readonly HttpClient _httpClient;

    public ControllersTest(TestServerEnvironment testServerEnvironment)
    {
        _httpClient = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task GetEmployeeReturnsOk()
    {
        var response = await _httpClient.GetAsync(_url + "Employee");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEmptyBulkPackagesReturnsOk()
    {
        var response = await _httpClient.GetAsync(_url + "EmptyBulkPackage");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetSpecialConditionReturnsOk()
    {
        var response = await _httpClient.GetAsync(_url + "SpecialCondition");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDeliveriesReturnsOk()
    {
        var response = await _httpClient.GetAsync(_url + "Delivery?top=1");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostMarkAsDeliveredReturnsNotFound()
    {
        var requestModel = new MarkDeliveredRequestModel
        {
            StartKms = 100000,
            EndKms = 100500,
            StartTime = DateTime.Today.AddDays(-1),
            EndTime = DateTime.Today,
            Breakages = null,
            Returns = null
        };
        var response = await _httpClient.PostAsync(_url + $"/-1/MarkAsDelivered", new StringContent(
            JsonConvert.SerializeObject(requestModel),
            Encoding.UTF8, MediaTypeNames.Application.Json));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostUploadMediaReturnsOk()
    {
        var reqModel = new UploadMediaRequestModel
        {
            DocumentNumber = 123,
            Mediae = new List<Media>
            {
                new()
                {
                    Content =
                        "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",
                    Filename = "iag.png"
                }
            }
        };
        var response = await _httpClient.PostAsync(_url + "Media/" + "UploadMedia", new StringContent(
            JsonConvert.SerializeObject(reqModel),
            Encoding.UTF8, MediaTypeNames.Application.Json));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(File.Exists("./123/iag.png"));
        Directory.Delete("./123", true);
    }
}