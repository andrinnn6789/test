using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using IAG.ControlCenter;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Settings;
using IAG.Infrastructure.Startup.Extensions;
using IAG.InstallClient.Resource;

using Microsoft.Extensions.Configuration;

namespace IAG.InstallClient.BusinessLogic;

public class CustomerManager : ICustomerManager
{
    private const string CustomerInfoFileName = "CustomerInfo.json";

    private readonly string _controlCenterBaseUrl;

    private string _customerInfoFilePath;
    private readonly JsonSerializerOptions _opt;

    public CustomerManager(IConfiguration configuration)
    {
        _controlCenterBaseUrl = configuration["ControlCenter:BaseUrl"];
        _opt = JsonConverterHelper.GetDefaulOption();
    }

    public async Task<CustomerInfo> GetCustomerInformationAsync(Guid id)
    {
        if (_controlCenterBaseUrl == null)
        {
            throw new LocalizableException(ResourceIds.ConfigCcBaseUrlMissingError);
        }

        var httpClient = new HttpClient() { BaseAddress = new Uri(_controlCenterBaseUrl) };
        var response = await httpClient.GetAsync(UrlHelper.Combine(ControlCenterEndpoints.Distribution, $"Customer/{id}"));
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new LocalizableException(ResourceIds.UnknownCustomerError);
        }
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            object message = string.IsNullOrEmpty(content) ? response.StatusCode : content;

            throw new LocalizableException(ResourceIds.GetCustomerError, message, (int)response.StatusCode);
        }

        var customerInfo = await JsonSerializer.DeserializeAsync<CustomerInfo>(await response.Content.ReadAsStreamAsync(), _opt);
        if (customerInfo != null)
        {
            customerInfo.ProductIds = null;
        }
                
        return customerInfo;
    }

    public async Task<CustomerInfo> GetCurrentCustomerInformationAsync()
    {
        if (!File.Exists(CustomerInfoFilePath))
        {
            return null;
        }

        await using var reader = File.OpenRead(CustomerInfoFilePath);
        return await JsonSerializer.DeserializeAsync<CustomerInfo>(reader, _opt);
    }

    public async Task SetCurrentCustomerInformationAsync(CustomerInfo customerInfo)
    {
        if (customerInfo == null)
        {
            File.Delete(CustomerInfoFilePath);  // Will not throw if file does not exists. According to documentation...
        }
        else
        {
            await using var writer = new StreamWriter(CustomerInfoFilePath, false);
            await JsonSerializer.SerializeAsync(writer.BaseStream, customerInfo);
        }
    }

    private string CustomerInfoFilePath => _customerInfoFilePath ??= Path.Combine(new SettingsFinder().GetSettingsPath(), CustomerInfoFileName);
}