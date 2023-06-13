using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using FluentAssertions;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.Test.Rest;

public class JsonResponseTest
{
    [Fact]
    public async void SimpleTest()
    {
        var testContent = "test";
        var httpResponse =
            new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(testContent) };
        var response = new JsonRestResponse(httpResponse);
        var content = await response.GetContent();
        await response.CheckResponse();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(testContent, content);
        Assert.NotNull(response.Headers);
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async void CheckResponseSuccessTest()
    {
        var httpResponse = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
        var response = new JsonRestResponse(httpResponse);
        await response.CheckResponse();
    }

    [Fact]
    public async void CheckResponseErrorTest()
    {
        var testContent = "Error";
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent(testContent),
            RequestMessage = new HttpRequestMessage
            {
                Content = new StringContent("bla"),
                RequestUri = new Uri("http://test.i-ag.ch/")
            }
        };
        var response = new JsonRestResponse(httpResponse);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(response.IsSuccessStatusCode);
        await Assert.ThrowsAsync<RestException>(() => response.CheckResponse());
    }

    [Fact]
    public async void GetDataTest()
    {
        var testBody = new TestBody() { Foo = "BAR", Answer = 42, Params = new List<object> { "FOO", 42, "BAR" } };
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent(JsonConvert.SerializeObject(testBody))
        };
        var response = new JsonRestResponse(httpResponse);
        var contentObject = await response.GetData<TestBody>();

        Assert.NotNull(response);
        Assert.NotNull(contentObject);
        testBody.Should().BeEquivalentTo(contentObject);
    }

    [Fact]
    public async void GetDataWithExceptionTest()
    {
        var testContent = "Something stupid...";
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent(testContent)
        };
        var response = new JsonRestResponse(httpResponse);

        Assert.NotNull(response);
        await Assert.ThrowsAsync<LocalizableException>(() => response.GetData<TestBody>());
    }

    private class TestBody
    {
        [UsedImplicitly]
        public string Foo { get; set; }

        [UsedImplicitly]
        public int Answer { get; set; }

        [UsedImplicitly]
        public List<object> Params { get; set; }
    }
}