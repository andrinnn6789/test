using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Controller;

[Collection("InfrastructureController")]
public class NumKeysODataControllerTest
{
    private readonly HttpClient _client;

    public NumKeysODataControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _client = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task CrudNumKey()
    {
        const string uri = "/odata/NumKeysOData";

        // POST empty
        var response = await _client.PostAsync(uri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // POST
        var numKeyInsert = new NumKey { Name = "test", Hint = "Hint" };
        response = await _client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(numKeyInsert), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var numKey = JsonConvert.DeserializeObject<NumKey>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(numKey);
        Assert.Equal(numKey.Name, numKeyInsert.Name);
        Assert.Equal(numKey.Hint, numKeyInsert.Hint);


        // PATCH empty
        response = await _client.PatchAsync(uri + "(" + numKey.Id + ")", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // PATCH not found
        response = await _client.PatchAsync(uri + "(999)",
            new StringContent(@"
                {
                    ""Name"": ""NewName""
                }
                ", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // PATCH
        response = await _client.PatchAsync(uri + "(" + numKey.Id +")",
            new StringContent(@"
                {
                    ""Name"": ""NewName""
                }
                ", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        response = await _client.GetAsync(uri + "(" + numKey.Id + ")");
        response.EnsureSuccessStatusCode();
        var numKeyPatched = JsonConvert.DeserializeObject<NumKey>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(numKeyPatched);
        Assert.Equal("NewName", numKeyPatched.Name);
        Assert.Equal(numKey.Hint, numKeyPatched.Hint);

        // PUT empty
        response = await _client.PutAsync(uri + "(123)", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // PUT not found
        response = await _client.PutAsync(
            uri + "(123)",
            new StringContent(JsonConvert.SerializeObject(new NumKey { Id = 123, Name = "test" }), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // PUT
        numKey.Name = "update";
        numKey.Hint = "xx";
        response = await _client.PutAsync(
            uri + "(" + numKey.Id + ")",
            new StringContent(JsonConvert.SerializeObject(numKey), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        response = await _client.PutAsync(
            uri + "(-1)",
            new StringContent(JsonConvert.SerializeObject(numKey), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        response = await _client.GetAsync(uri + "(" + numKey.Id + ")");
        var numKeyUpdated = JsonConvert.DeserializeObject<NumKey>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(numKeyUpdated);
        Assert.Equal(numKey.Name, numKeyUpdated.Name);
        Assert.Equal(numKey.Hint, numKeyUpdated.Hint);

        // DELETE
        response = await _client.DeleteAsync(uri + "(" + numKey.Id + ")");
        response.EnsureSuccessStatusCode();
        response = await _client.DeleteAsync(uri + "(" + numKey.Id + ")");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // GET
        response = await _client.GetAsync(uri + "(" + numKey.Id + ")");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        response = await _client.GetAsync(uri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}