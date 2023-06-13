using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Globalisation.Controller;

[Collection("InfrastructureController")]
public class CultureControllerTest
{
    private const string Uri = InfrastructureEndpoints.Resource + "Culture/";
    private readonly HttpClient _client;

    public CultureControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _client = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task CrudCultures()
    {
        var cultId = Guid.NewGuid();
        var culture = new Culture { Id = cultId, Name = "jp" };

        // POST
        var response = await _client.PostAsync(Uri,
            new StringContent(@"
                {
                    ""Id"":  """ + cultId + @""",
                    ""Name"": ""jp""
                }
                ", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var cultureInserted = JsonConvert.DeserializeObject<Culture>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(cultureInserted);
        Assert.Equal(culture.Name, cultureInserted.Name);

        // PUT
        cultureInserted.Name = "jp-jp";
        response = await _client.PutAsync(Uri + cultureInserted.Id,
            new StringContent(JsonConvert.SerializeObject(cultureInserted), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // GET
        response = await _client.GetAsync(Uri + cultureInserted.Id);
        response.EnsureSuccessStatusCode();
        var cultureUpdated = JsonConvert.DeserializeObject<Culture>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(cultureUpdated);
        Assert.Equal(cultureInserted.Name, cultureUpdated.Name);

        // $expand
        var resId = Guid.NewGuid();
        response = await _client.PostAsync(InfrastructureEndpoints.Resource + "Resource",
            new StringContent(JsonConvert.SerializeObject(
                new Infrastructure.Globalisation.Model.Resource { Id = resId, Name = "test" }), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var resInserted = JsonConvert.DeserializeObject<Infrastructure.Globalisation.Model.Resource>(await response.Content.ReadAsStringAsync());
        var transId = Guid.NewGuid();
        response = await _client.PostAsync(InfrastructureEndpoints.Resource + "Translation",
            new StringContent(@"
                {
                    ""Id"": """ + transId + @""",
                    ""Value"": ""TestExpand"",
                    ""ResourceId"": """ + resInserted?.Id + @""",
                    ""CultureId"": """ + cultureInserted.Id + @"""
                }
                ", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        // DELETE
        response = await _client.DeleteAsync(Uri + cultureInserted.Id);
        response.EnsureSuccessStatusCode();

        // GET
        response = await _client.GetAsync(Uri + cultureInserted.Id);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}