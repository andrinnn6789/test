using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;

using IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.Jira;

using Newtonsoft.Json;

using RestClient = IAG.Infrastructure.Rest.RestClient;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.Jira;

public class VersionSyncJiraConnector: RestClient
{

    private readonly JsonSerializerSettings _jsonSerializerSetting = new()
    {
        NullValueHandling = NullValueHandling.Include,
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new JsonPropertyAnnotationContractResolver(),
    };

    public VersionSyncJiraConnector(IHttpConfig config, IRequestResponseLogger logger) : base(config, logger)
    {
    }

    public async Task<Version> CreateVersion(CreateVersionRequestModel version)
    {
        var request = new JsonRestRequest(HttpMethod.Post, "version");
        request.SetJsonBody(version, _jsonSerializerSetting);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var addedVersion = await response.GetData<Version>();
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

    public async Task<Version[]> GetVersionsFromProject(int projectId)
    {
        var request = new JsonRestRequest(HttpMethod.Get, $"project/{projectId}/versions");

            var response = await ExecuteAsync(request);
            await response.CheckResponse();

            var versions = await response.GetData<Version[]>();
            return versions;
    }
}