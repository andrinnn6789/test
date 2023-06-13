using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.MockHost;

using Moq;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Rest;

public class RestClientAndLoggerTest
{
    private readonly IHttpConfig _httpConfig;
    private readonly RequestResponseLogger _logger;
    private readonly List<MessageStructure> _messages;

    public RestClientAndLoggerTest()
    {
        _messages = new List<MessageStructure>();
        var messageLogger = new Mock<IMessageLogger>();
        messageLogger.Setup(m => m.AddMessage(It.IsAny<MessageStructure>())).Callback<MessageStructure>((msg) => _messages.Add(msg));

        _logger = new RequestResponseLogger(messageLogger.Object);

        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(typeof(RestClientAndLoggerTest).Namespace + ".RequestMock.json", testPort);

        _httpConfig = new HttpConfig
        {
            BaseUrl = $"http://localhost:{testPort}/"
        };
    }

    [Fact]
    public async Task GetRequestWithoutLoggerTest()
    {
        var restClient = new RestClient(_httpConfig);
        var response = await restClient.GetAsync<JObject>("/todos/1");

        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetRequestWithLoggerTest()
    {
        var restClient = new RestClient(_httpConfig, _logger);
        var testUrl = "/todos/1";
        var response = await restClient.GetAsync<JObject>(testUrl);

        Assert.NotNull(response);
        Assert.Equal(2, _messages.Count);

        var requestLogMessage = _messages[0];
        Assert.NotNull(requestLogMessage);
        Assert.NotNull(requestLogMessage.Params);
        Assert.All(requestLogMessage.Params, Assert.NotNull);
        Assert.Single(requestLogMessage.Params);

        var responseLogMessage = _messages[1];
        Assert.NotNull(responseLogMessage);
        Assert.NotNull(responseLogMessage.Params);
        Assert.All(responseLogMessage.Params, Assert.NotNull);
        Assert.Single(responseLogMessage.Params);
    }

    [Fact]
    public async Task FailedGetRequestWithLoggerTest()
    {
        var restClient = new RestClient(_httpConfig, _logger);
        var testUrl = "/todos/not_existing_id";

        await Assert.ThrowsAsync<RestException>(() => restClient.GetAsync<JObject>(testUrl));

        Assert.Equal(2, _messages.Count);

        var requestLogMessage = _messages[0];
        Assert.NotNull(requestLogMessage);
        Assert.NotNull(requestLogMessage.Params);
        Assert.All(requestLogMessage.Params, Assert.NotNull);
        Assert.Single(requestLogMessage.Params);

        var responseLogMessage = _messages[1];
        Assert.NotNull(responseLogMessage);
        Assert.NotNull(responseLogMessage.Params);
        Assert.All(responseLogMessage.Params, Assert.NotNull);
        Assert.Single(responseLogMessage.Params);
    }

    [Fact]
    public async Task PostWithResponse()
    {
        var client = new RestClient(_httpConfig);
        var request = new JsonRestRequest(HttpMethod.Post, "/posts")
        {
            Content = new StringContent(@"
                    {
                        ""title"": ""foo"",
                        ""body"": ""bar"",
                        ""userId"": 1
                    }
                ")
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypes.ApplicationJson);
        var response = await client.PostAsync<JObject>(request);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task PatchWithResponse()
    {
        var client = new RestClient(_httpConfig);
        var request = new JsonRestRequest(HttpMethod.Patch, "/patches")
        {
            Content = new StringContent(@"
                    {
                        ""title"": ""foo"",
                        ""body"": ""bar"",
                        ""userId"": 1
                    }
                ")
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypes.ApplicationJson);
        var response = await client.PatchAsync<JObject>(request);

        Assert.NotNull(response);
    }
}