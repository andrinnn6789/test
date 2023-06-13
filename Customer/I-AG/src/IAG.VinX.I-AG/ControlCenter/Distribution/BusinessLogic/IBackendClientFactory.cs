using IAG.Infrastructure.Rest;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public interface IBackendClientFactory
{
    T CreateRestClient<T>(string endpoint) where T: RestClient;
}