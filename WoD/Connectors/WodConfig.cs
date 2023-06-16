using System.Collections.Generic;
using System.Net.Mime;

using IAG.Common.Dto;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Authentication;

using Microsoft.Net.Http.Headers;

namespace IAG.Common.WoD.Connectors;

public class WodConfig
{
    private readonly BasicAuthentication _basicAuthentication = new();

    private const string TenantHeader = "X-WOD-Tenant";
    private const string JobTypeHeader = "X-WOD-JobType";

    public WodConfig()
    {
        HttpConfig = new HttpConfig
        {
            HttpHeaders = new Dictionary<string, string>
            {
                { HeaderNames.Accept, $"{MediaTypeNames.Application.Pdf}, {MediaTypeNames.Text.Plain}"},
                { TenantHeader, string.Empty },
                { JobTypeHeader, string.Empty }
            },
            Authentication = _basicAuthentication
        };
    }

    public HttpConfig HttpConfig { get; }

    public ProviderSetting ProviderSetting
    {
        set
        {
            if (!value.BaseUrl.EndsWith("/"))
            {
                value.BaseUrl += "/";
            }

            HttpConfig.BaseUrl = value.BaseUrl;
            _basicAuthentication.User = value.UserName;
            _basicAuthentication.Password = value.Password;
            HttpConfig.HttpHeaders[TenantHeader] = value.ParticipantId;
        }
    }

    public string JobType
    {
        set => HttpConfig.HttpHeaders[JobTypeHeader] = value;
    }
}