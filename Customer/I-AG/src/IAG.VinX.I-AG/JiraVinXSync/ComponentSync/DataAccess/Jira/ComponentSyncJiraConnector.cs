using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.Jira;

using Newtonsoft.Json;

using RestClient = IAG.Infrastructure.Rest.RestClient;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.Jira;

public class ComponentSyncJiraConnector: RestClient
{
    private readonly JsonSerializerSettings _jsonSerializerSetting = new()
    {
        NullValueHandling = NullValueHandling.Include,
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new JsonPropertyAnnotationContractResolver(),
    };

    public ComponentSyncJiraConnector(IHttpConfig config, IRequestResponseLogger logger) : base(config, logger)
    {
    }

    public async Task<Component> CreateComponent(CreateComponentRequestModel component)
    {
        var request = new JsonRestRequest(HttpMethod.Post, "component");
        request.SetJsonBody(component, _jsonSerializerSetting);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var addedVersion = await response.GetData<Component>();
        return addedVersion;
    }

    public async Task<Project[]> GetProjects()
    {
        var request = new JsonRestRequest(HttpMethod.Get, "project");

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var projects = await response.GetData<Project[]>();
        return projects;
    }

    public async Task<Component[]> GetComponentsFromProject(int projectId)
    {
        var request = new JsonRestRequest(HttpMethod.Get, $"project/{projectId}/components");

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var components = await response.GetData<Component[]>();
        return components;
    }
}