using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Globalisation.Controller;

[Collection("InfrastructureController")]
public class ResourceControllerTest
{
    private const string Uri = InfrastructureEndpoints.Resource + "Resource/";
    private readonly HttpClient _client;

    public ResourceControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _client = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task CrudResources()
    {
        var resId = Guid.NewGuid();
        var res = new Infrastructure.Globalisation.Model.Resource { Id = resId, Name = "test" };

        // POST
        var json = JsonConvert.SerializeObject(res);
        var response = await _client.PostAsync(Uri, new StringContent(json, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var resInserted = JsonConvert.DeserializeObject<Infrastructure.Globalisation.Model.Resource>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(resInserted);
        Assert.Equal(res.Name, resInserted.Name);

        // PUT
        resInserted.Name = "update";
        response = await _client.PutAsync(Uri + resInserted.Id,
            new StringContent(JsonConvert.SerializeObject(resInserted), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // GET
        response = await _client.GetAsync(Uri + resInserted.Id);
        response.EnsureSuccessStatusCode();
        var resUpdated = JsonConvert.DeserializeObject<Infrastructure.Globalisation.Model.Resource>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(resUpdated);
        Assert.Equal(resInserted.Name, resUpdated.Name);

        // DELETE
        response = await _client.DeleteAsync(Uri + resInserted.Id);
        response.EnsureSuccessStatusCode();

        // GET
        response = await _client.GetAsync(Uri + resInserted.Id);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}