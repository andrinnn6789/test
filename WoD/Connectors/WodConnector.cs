using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

using IAG.Common.Resource;
using IAG.Common.WoD.Dto;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Rest;

using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace IAG.Common.WoD.Connectors;

public class WodConnector: IWodConnector
{
    private readonly IRequestResponseLogger _logger;
    private readonly WodConfig _config;

    private const string DocEndpoint = "jobs/produceDocument";
    private const string ConnectionTestEndpoint = "trouble/processing";

    public WodConnector(ILogger<WodConnector> logger, IWodConfigLoader wodConfigLoader)
    {
        _logger = new RequestResponseLogger(logger); 
        _config = new WodConfig
        {
            ProviderSetting = wodConfigLoader.ProviderSetting()
        };
    }

    public async Task<byte[]> SubmitJob(byte[] zip, string jobType)
    {
        _config.JobType = jobType;
        var client = new RestClient(_config.HttpConfig, _logger);
        client.DefaultRequestHeaders.Authorization = _config.HttpConfig.Authentication.GetAuthorizationHeader();
        var request = new HttpRequestMessage
        {
            RequestUri = client.BaseAddress != null ? new Uri(client.BaseAddress, DocEndpoint) : new Uri(DocEndpoint),
            Method = HttpMethod.Post,
            Content = new ByteArrayContent(zip, 0, zip.Length)
            {
                Headers =
                {
                    { HeaderNames.ContentType, MediaTypeNames.Application.Zip }
                }
            }
        };

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            throw new LocalizableException(ResourceIds.WoDConnectionFailed, e);
        }

        return await response.Content.ReadAsByteArrayAsync();
    }

    public Task<WodConnectionResult<LocalizableParameter>> CheckWodConnectionAsync()
    {
        return CheckWodConnectionAsync<WodConnectionResult<LocalizableParameter>>();
    }

    private async Task<T> CheckWodConnectionAsync<T>()
        where T : WodConnectionResult<LocalizableParameter>, new()
    {
        try
        {
            var client = new RestClient(_config.HttpConfig, _logger);
            client.DefaultRequestHeaders.Authorization = _config.HttpConfig.Authentication.GetAuthorizationHeader();
            var response = await client.GetAsync(ConnectionTestEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                return new T
                {
                    Success = false,
                    Info = new LocalizableParameter(
                        ResourceIds.WoDConnectionFailed,
                        $"{response.StatusCode} {response.Content.ReadAsStringAsync().Result}")
                };
            }

            return new T
            {
                Success = true,
                Info = new LocalizableParameter("Ok")
            };
        }
        catch (Exception ex)
        {
            return new T
            {
                Success = false,
                Info = new LocalizableParameter(ResourceIds.WoDConnectionFailed, ex.Message)
            };
        }
    }
}