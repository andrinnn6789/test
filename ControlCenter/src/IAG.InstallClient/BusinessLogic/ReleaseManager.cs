using System;
using System.Collections.Generic;
using System.IO;
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

public class ReleaseManager : IReleaseManager
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _opt;

    public ReleaseManager(IConfiguration configuration)
    {
        var controlCenterBaseUrl = configuration["ControlCenter:BaseUrl"];
        if (controlCenterBaseUrl == null)
        {
            throw new LocalizableException(ResourceIds.ConfigCcBaseUrlMissingError);
        }

        _httpClient = new HttpClient { BaseAddress = new Uri(controlCenterBaseUrl) };
        _opt = JsonConverterHelper.GetDefaulOption();
    }

    public async Task<IEnumerable<ProductInfo>> GetProductsAsync(Guid customerId)
    {
        var response = await _httpClient.GetAsync(UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{customerId}/Product"));
        response.EnsureSuccessStatusCode();
        var products = await JsonSerializer.DeserializeAsync<IEnumerable<ProductInfo>>(await response.Content.ReadAsStreamAsync(), _opt);

        return products;
    }

    public async Task<IEnumerable<ReleaseInfo>> GetReleasesAsync(Guid customerId, Guid productId)
    {
        var response = await _httpClient.GetAsync(UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{customerId}/Product/{productId}/Release"));
        response.EnsureSuccessStatusCode();
        var releases = await JsonSerializer.DeserializeAsync<IEnumerable<ReleaseInfo>>(await response.Content.ReadAsStreamAsync(), _opt);

        return releases;
    }

    public async Task<IEnumerable<FileMetaInfo>> GetReleaseFilesAsync(Guid customerId, Guid productId, Guid releaseId)
    {
        var response = await _httpClient.GetAsync(UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{customerId}/Product/{productId}/Release/{releaseId}/File"));
        response.EnsureSuccessStatusCode();
        var files = await JsonSerializer.DeserializeAsync<IEnumerable<FileMetaInfo>>(await response.Content.ReadAsStreamAsync(), _opt);

        return files;
    }

    public async Task<Stream> GetFileContentStreamAsync(Guid customerId, Guid fileId)
    {
        var response = await _httpClient.GetAsync(UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{customerId}/File/{fileId}"));
        response.EnsureSuccessStatusCode();
            
        return await response.Content.ReadAsStreamAsync();
    }
}