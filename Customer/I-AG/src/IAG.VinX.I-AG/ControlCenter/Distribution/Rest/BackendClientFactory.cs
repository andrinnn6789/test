using System;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public class BackendClientFactory : IBackendClientFactory
{
    private readonly BackendConfig _backendConfig;
    private readonly IControlCenterTokenRequest _tokenRequest;
    private readonly ILogger _logger;
    private IHttpConfig _httpConfig;

    public BackendClientFactory(BackendConfig backendConfig, IControlCenterTokenRequest tokenRequest, ILogger logger)
    {
        _backendConfig = backendConfig;
        _tokenRequest = tokenRequest;
        _logger = logger;
    }

    public T CreateRestClient<T>(string endpoint) where T: RestClient
    {
        _httpConfig = _httpConfig ?? GetHttpConfig(endpoint);

        var client = (T)Activator.CreateInstance(typeof(T), _httpConfig, GetRequestResponseLogger());
        // ReSharper disable once PossibleNullReferenceException
        client.Timeout = _backendConfig.RequestTimeout;
        return client;
    }

    private IHttpConfig GetHttpConfig(string endpoint)
    {
        try
        {
            if (string.IsNullOrEmpty(_backendConfig.Username))
            {
                return new HttpConfig { BaseUrl = new Uri(new Uri(_backendConfig.UrlControlCenter), endpoint).ToString() };
            }
            return _tokenRequest.GetConfig(_backendConfig, endpoint, GetRequestResponseLogger());
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.AuthenticationError, ex);
        }
    }

    private RequestResponseLogger GetRequestResponseLogger()
    {
        return _logger != null ? new RequestResponseLogger(_logger) : null;
    }
}