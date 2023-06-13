using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using IAG.ControlCenter;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Startup.Extensions;
using IAG.InstallClient.Resource;

using Microsoft.Extensions.Configuration;

namespace IAG.InstallClient.BusinessLogic;

public class LinkListManager : ILinkListManager
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _opt;

    public LinkListManager(IConfiguration configuration)
    {
        var controlCenterBaseUrl = configuration["ControlCenter:BaseUrl"];
        if (controlCenterBaseUrl == null)
        {
            throw new LocalizableException(ResourceIds.ConfigCcBaseUrlMissingError);
        }

        _httpClient = new HttpClient { BaseAddress = new Uri(controlCenterBaseUrl) };
        _opt = JsonConverterHelper.GetDefaulOption();
    }

    public async Task<IEnumerable<LinkInfo>> GetLinksAsync(Guid customerId)
    {
        var response = await _httpClient.GetAsync(UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{customerId}/Link"));
        response.EnsureSuccessStatusCode();
        var links = await JsonSerializer.DeserializeAsync<IEnumerable<LinkInfo>>(await response.Content.ReadAsStreamAsync(), _opt);

        return links;
    }
}