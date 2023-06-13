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
public class TranslationControllerTest
{
    private const string Uri = InfrastructureEndpoints.Resource + "Translation/";
    private readonly HttpClient _client;

    public TranslationControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _client = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task CrudTranslation()
    {
        // Arrange
        var transId = Guid.NewGuid();
        var resId = Guid.NewGuid();
        var cultId = Guid.NewGuid();
        var resource = new Infrastructure.Globalisation.Model.Resource { Id = resId, Name = "test"};
        await _client.PostAsync("/api/Resources", new StringContent(JsonConvert.SerializeObject(resource), Encoding.UTF8, "application/json"));

        // POST
        var translation = new Translation { ResourceId = resId, CultureId = cultId, Value = "test" };
        var response = await _client.PostAsync(Uri,
            new StringContent(@"
                {
                    ""Id"": """ + transId+ @""",
                    ""ResourceId"": """ + resId + @""",
                    ""CultureId"":  """ + cultId + @""",
                    ""Value"": ""test""
                }
                ", Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var translationInserted = JsonConvert.DeserializeObject<Translation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(translationInserted);
        Assert.Equal(translation.Value, translationInserted.Value);

        // PUT
        translationInserted.Value = "update";
        response = await _client.PutAsync(Uri + translationInserted.Id,
            new StringContent(JsonConvert.SerializeObject(translationInserted), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // GET
        response = await _client.GetAsync(Uri + translationInserted.Id);
        response.EnsureSuccessStatusCode();
        var translationUpdated = JsonConvert.DeserializeObject<Translation>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(translationUpdated);
        Assert.Equal(translationInserted.Value, translationUpdated.Value);

        // DELETE
        response = await _client.DeleteAsync(Uri + translationInserted.Id);
        response.EnsureSuccessStatusCode();

        // GET
        response = await _client.GetAsync(Uri + translationInserted.Id);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}