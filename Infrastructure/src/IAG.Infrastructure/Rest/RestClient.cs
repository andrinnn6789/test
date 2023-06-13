using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Net.Http.Headers;

namespace IAG.Infrastructure.Rest;

public class RestClient : HttpClient
{
    protected IRequestResponseLogger Logger { get; }

    public RestClient(IHttpConfig config, IRequestResponseLogger logger = null)
    {
        Init(config);
        Logger = logger;
    }

    public RestClient(IHttpConfig config, HttpProxy proxy, IRequestResponseLogger logger = null) : base(new HttpClientHandler { Proxy = proxy, UseProxy = true })
    {
        Init(config);
        Logger = logger;
    }

    public async Task<T> GetAsync<T>(string resourcePath)
    {
        var request = new JsonRestRequest(HttpMethod.Get, resourcePath);
        var response = await ExecuteAsync(request);
        await response.CheckResponse();
        return await response.GetData<T>();
    }

    public async Task<T> PostAsync<T>(JsonRestRequest request)
    {
        var response = await ExecuteAsync(request);
        await response.CheckResponse();
        return await response.GetData<T>();
    }

    public async Task<T> PatchAsync<T>(JsonRestRequest request)
    {
        var response = await ExecuteAsync(request);
        await response.CheckResponse();
        return await response.GetData<T>();
    }

    public virtual async Task<IRestResponse> ExecuteAsync(JsonRestRequest request)
    {
        PrepareRequest(request);
        var httpResponse = await SendAsync(request);
        return new JsonRestResponse(httpResponse);
    }

    // intercept direct calls to SendAsync in httpClient to log all traffic
    // clean way would be a specific HttpMessageHandler to intercept all calls
    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
    }

    [ExcludeFromCodeCoverage]  // not yet used
    public new Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }

    public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
        CancellationToken cancellationToken)
    {
        Logger?.LogRequest(BaseAddress?.AbsoluteUri, request);
        var httpResponse = await base.SendAsync(request, completionOption, cancellationToken);
        Logger?.LogResponse(httpResponse);
        return httpResponse;
    }

    protected IHttpConfig Configuration { get; private set; }

    protected void PrepareRequest(HttpRequestMessage request)
    {
        if (Configuration.Authentication != null)
        {
            request.Headers.Authorization = Configuration.Authentication.GetAuthorizationHeader();
        }
    }

    private void Init(IHttpConfig config)
    {
        var baseUrl = config.BaseUrl;
        if (baseUrl == null)
            throw new System.Exception("Url is empty in http-config");

        if (!baseUrl.EndsWith("/"))
        {
            baseUrl += "/";
        }

        Configuration = config;
        BaseAddress = new Uri(baseUrl);

        var acceptJsonAdded = false;
        if (config.HttpHeaders?.Count > 0)
        {
            foreach (var httpHeader in config.HttpHeaders)
            {
                DefaultRequestHeaders.Add(httpHeader.Key, httpHeader.Value);
                if (string.Equals(httpHeader.Key, HeaderNames.Accept, StringComparison.InvariantCultureIgnoreCase)
                    && httpHeader.Value.StartsWith(
                        ContentTypes.ApplicationJson,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    acceptJsonAdded = true;
                }
            }
        }

        if (!acceptJsonAdded)
        {
            DefaultRequestHeaders.Add(HeaderNames.Accept, ContentTypes.ApplicationJson);
        }

        /* Code für Fehlerbehandlung Zertifikate
        System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, x509Certificate, chain, errors) =>
            {
                if (errors == System.Net.Security.SslPolicyErrors.None)
                    return true;

                // Behandlung fehlerhafte/eigene Zertifikate...
                return false;
            };
        */
    }
}