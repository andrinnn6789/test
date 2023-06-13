using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using FluentAssertions;

using IAG.Infrastructure.Rest;

using JetBrains.Annotations;

using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Xunit;

namespace IAG.Infrastructure.Test.Rest;

public class JsonRequestTest
{
    [Fact]
    public void AbsoulutUriTest()
    {
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/test");

        Assert.NotNull(request);
        Assert.NotNull(request.RequestUri);
        Assert.Equal("http://test.i-ag.ch/test", request.RequestUri.ToString());
    }

    [Fact]
    public void AbsoulutUriResourceTest()
    {
        var request = new JsonRestRequest(HttpMethod.Get, new Uri("http://test.i-ag.ch/test"));

        Assert.NotNull(request);
        Assert.NotNull(request.RequestUri);
        Assert.Equal("http://test.i-ag.ch/test", request.RequestUri.ToString());
    }

    [Fact]
    public void RelativeUriTest()
    {
        var request = new JsonRestRequest(HttpMethod.Get, "test");

        Assert.NotNull(request);
        Assert.NotNull(request.RequestUri);
        Assert.Equal("test", request.RequestUri.ToString());
    }

    [Fact]
    public void UrlSegmentTest()
    {
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetUrlSegment("id", 123);
        request.SetUrlSegment("foo", "bar");
        string url1 = request.RequestUri?.ToString();
        request.SetUrlSegment("id", 42);
        request.SetUrlSegment("foo", "foo");
        string url2 = request.RequestUri?.ToString();

        Assert.Equal("http://test.i-ag.ch/123/bar", url1);
        Assert.Equal("http://test.i-ag.ch/42/foo", url2);
    }

    [Fact]
    public void QueryTest()
    {
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/");
        request.SetQueryParameter("foo", "bar");
        request.SetQueryParameter("answer", 42);
        var queryString = QueryHelpers.ParseQuery(request.RequestUri?.Query ?? string.Empty);

        Assert.NotNull(request);
        Assert.NotNull(request.RequestUri);
        Assert.StartsWith("http://test.i-ag.ch/", request.RequestUri.ToString());
        Assert.NotNull(queryString);
        Assert.NotEmpty(queryString);
        Assert.True(queryString.ContainsKey("foo"));
        Assert.True(queryString.ContainsKey("answer"));
        Assert.Equal("bar", queryString["foo"]);
        Assert.Equal("42", queryString["answer"]);
    }

    [Fact]
    public void ComplexUrlTest()
    {
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetUrlSegment("id", 123);
        request.SetUrlSegment("foo", "bar");
        request.SetQueryParameter("answer", 42);
        string url1 = request.RequestUri?.ToString();
        request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetQueryParameter("answer", 42);
        request.SetUrlSegment("id", 123);
        request.SetUrlSegment("foo", "bar");
        string url2 = request.RequestUri?.ToString();

        Assert.Equal(url1, url2);
    }


    [Fact]
    public async void BodyTest()
    {
        var testBody = new TestBody() { Foo = "bar", Answer = 42, Params = new List<object> { "foo", 42, "bar" } };
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetJsonBody(testBody);

        Assert.NotNull(request);
        Assert.NotNull(request.Content);
        Assert.IsType<StringContent>(request.Content);
        Assert.NotNull(request.Content.Headers);
        Assert.NotNull(request.Content.Headers.ContentType);
        Assert.Equal(ContentTypes.ApplicationJson, request.Content.Headers.ContentType.MediaType);
        testBody.Should().BeEquivalentTo(JsonConvert.DeserializeObject<TestBody>(await request.Content.ReadAsStringAsync()));
    }

    [Fact]
    public async void BodyWithJsonSettingsTest()
    {
        var testBody = new TestBody
        {
            Id = "ABC",
            Foo = "BAR",
            Answer = 42,
            Params = new List<object> { "FOO", 42, "BAR" },
            CamelCaseSpecialTest = string.Empty
        };
        var jsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new ScreamingContractResolver()
        };
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetJsonBody(testBody, jsonSettings);

        Assert.NotNull(request);
        Assert.NotNull(request.Content);
        Assert.IsType<StringContent>(request.Content);
        Assert.NotNull(request.Content.Headers);
        Assert.NotNull(request.Content.Headers.ContentType);
        Assert.Equal(ContentTypes.ApplicationJson, request.Content.Headers.ContentType.MediaType);
        Assert.True((await request.Content.ReadAsStringAsync()).All(c => !char.IsLetter(c) || char.IsUpper(c)));
    }

    [Fact]
    public async void BodyWithPropertyAnnotationContractResolverTest()
    {
        var testBody = new TestBody() { CamelCaseSpecialTest = "42" };
        var jsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new JsonPropertyAnnotationContractResolver()
        };
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetJsonBody(testBody, jsonSettings);

        Assert.NotNull(request);
        Assert.NotNull(request.Content);
        Assert.IsType<StringContent>(request.Content);
        Assert.NotNull(request.Content.Headers);
        Assert.NotNull(request.Content.Headers.ContentType);
        Assert.Equal(ContentTypes.ApplicationJson, request.Content.Headers.ContentType.MediaType);
        Assert.Contains("CamelCaseSPECIALTest", await request.Content.ReadAsStringAsync());
    }

    [Fact]
    public async void BodyWithLowercasePropertyContractResolverTest()
    {
        var testBody = new TestBody() { CamelCaseSpecialTest = "42" };
        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new JsonLowercasePropertyContractResolver()
        };
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetJsonBody(testBody, jsonSettings);

        Assert.NotNull(request);
        Assert.NotNull(request.Content);
        Assert.IsType<StringContent>(request.Content);
        Assert.NotNull(request.Content.Headers);
        Assert.NotNull(request.Content.Headers.ContentType);
        Assert.Equal(ContentTypes.ApplicationJson, request.Content.Headers.ContentType.MediaType);
        Assert.Contains("camelcasespecialtest", await request.Content.ReadAsStringAsync());
    }

    [Fact]
    public async void SuppressKeySerializeContractResolverTest()
    {
        var testBody = new TestBody() { Id = "42" };
        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new SuppressKeySerializeContractResolver()
        };
        var request = new JsonRestRequest(HttpMethod.Get, "http://test.i-ag.ch/{id}/{foo}");
        request.SetJsonBody(testBody, jsonSettings);

        Assert.NotNull(request);
        Assert.NotNull(request.Content);
        Assert.IsType<StringContent>(request.Content);
        Assert.DoesNotContain("42", await request.Content.ReadAsStringAsync());
    }

    private class TestBody
    {
        [UsedImplicitly]
        public string Id { get; set; }

        [UsedImplicitly]
        public string Foo { get; set; }

        [UsedImplicitly]
        public int Answer { get; set; }

        [UsedImplicitly]
        public List<object> Params { get; set; }

        [JsonProperty("CamelCaseSPECIALTest")]
        public string CamelCaseSpecialTest { get; set; }
    }

    private class ScreamingContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string key)
        {
            return key.ToUpper();
        }
    }
}