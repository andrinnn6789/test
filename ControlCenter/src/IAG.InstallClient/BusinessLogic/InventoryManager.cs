using System;
using System.Net.Http;
using System.Text;
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

public class InventoryManager : IInventoryManager
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _opt;

    public InventoryManager(IConfiguration configuration)
    {
        var controlCenterBaseUrl = configuration["ControlCenter:BaseUrl"];
        if (controlCenterBaseUrl == null)
        {
            throw new LocalizableException(ResourceIds.ConfigCcBaseUrlMissingError);
        }

        _httpClient = new HttpClient { BaseAddress = new Uri(controlCenterBaseUrl) };
        _opt = JsonConverterHelper.GetDefaulOption();
    }

    public async Task<InstallationInfo> RegisterInstallationAsync(Guid customerId, InstallationRegistration installationRegistration)
    {
        var response = _httpClient.PostAsync(
            UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{customerId}/Installation"),
            new StringContent(JsonSerializer.Serialize(installationRegistration), Encoding.UTF8, "application/json")).Result;
        response.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<InstallationInfo>(await response.Content.ReadAsStreamAsync(), _opt);
    }

    public Task<InstallationInfo> DeRegisterInstallationAsync(Guid customerId, string instanceName)
    {
        var installation = new InstallationRegistration
        {
            InstanceName = instanceName
        };

        return RegisterInstallationAsync(customerId, installation);
    }
}