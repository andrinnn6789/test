using System.Net;
using System.Net.Http;

using Castle.Components.DictionaryAdapter;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;
using IAG.Infrastructure.Rest.Atlas.Dto;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.Test.Rest.Atlas;

public class AtlasRestResponseTest
{
    [Fact]
    public async void StructuredErrorMessageTest()
    {
        var testErrorMsg = "General Server Error";
        var testResponse = new AtlasResponse<BaseResource>();
        testResponse.Resource = new EditableList<Resource<BaseResource>>()
        {
            new() { Message = testErrorMsg }
        };
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(JsonConvert.SerializeObject(testResponse))
        };
        var response = new AtlasRestResponse(httpResponse);
        var errorMsg = await response.GetErrorMessage();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(testErrorMsg, errorMsg);
        await Assert.ThrowsAsync<RestException>(() => response.CheckResponse());
    }

    [Fact]
    public async void DefaultErrorMessageTest()
    {
        var testErrorMsg = "General Server Error";
        var httpResponse =
            new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(testErrorMsg) };
        var response = new AtlasRestResponse(httpResponse);
        var errorMsg = await response.GetErrorMessage();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(testErrorMsg, errorMsg);
        await Assert.ThrowsAsync<RestException>(() => response.CheckResponse());
    }
}