using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;

namespace IAG.Infrastructure.Rest;

public class JsonRestRequest : HttpRequestMessage
{
    // ReSharper disable once StyleCop.SA1401
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public static JsonSerializerSettings DefaultJsonSerializerSetting = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new JsonPropertyAnnotationContractResolver()
    };

    public JsonRestRequest(HttpMethod method, string resource) : base(method, resource)
    {
        OriginalUrl = resource;
        UrlSegments = new Dictionary<string, string>();
    }

    public JsonRestRequest(HttpMethod method, Uri resource) : base(method, resource)
    {
        OriginalUrl = resource.OriginalString;
        UrlSegments = new Dictionary<string, string>();
    }

    protected string OriginalUrl { get; private set; }

    protected Dictionary<string, string> UrlSegments { get; }

    public JsonRestRequest SetQueryParameter(string name, string value)
    {
        OriginalUrl = QueryHelpers.AddQueryString(OriginalUrl, name, value);

        UpdateRequestUri();

        return this;
    }

    public JsonRestRequest SetQueryParameter(string name, object value)
    {
        return SetQueryParameter(name, value.ToString());
    }

    public JsonRestRequest SetUrlSegment(string name, string value)
    {
        UrlSegments[name] = value;

        UpdateRequestUri();

        return this;
    }

    public JsonRestRequest SetUrlSegment(string name, object value)
    {
        return SetUrlSegment(name, value.ToString());
    }

    public JsonRestRequest SetJsonBody(object body)
    {
        return SetJsonBody(body, DefaultJsonSerializerSetting);
    }

    public JsonRestRequest SetJsonBody(object body, JsonSerializerSettings serializerSettings)
    {
        string content = JsonConvert.SerializeObject(body, serializerSettings);
        Content = new StringContent(content);
        Content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypes.ApplicationJson);
        return this;
    }

    private void UpdateRequestUri()
    {
        string uri = OriginalUrl;
        foreach (KeyValuePair<string, string> urlSegment in UrlSegments)
        {
            string keyPlaceholder = $"{{{urlSegment.Key}}}";
            uri = uri.Replace(keyPlaceholder, urlSegment.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        RequestUri = new Uri(uri, RequestUri?.IsAbsoluteUri == true ? UriKind.Absolute : UriKind.Relative);
    }
}