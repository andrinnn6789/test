using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;

using IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.Jira;

using Newtonsoft.Json;

using RestClient = IAG.Infrastructure.Rest.RestClient;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.Jira;

public class WorklogSyncJiraConnector: RestClient
{
       
    private readonly JsonSerializerSettings _jsonSerializerSetting = new()
    {
        NullValueHandling = NullValueHandling.Include,
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new JsonPropertyAnnotationContractResolver(),
    };

    public WorklogSyncJiraConnector(IHttpConfig config, IRequestResponseLogger logger) : base(config, logger)
    {
    }

    public async Task<Worklog[]> GetWorklogs(GetWorklogsRequestModel requestModel)
    {
        var request = new JsonRestRequest(HttpMethod.Post, "worklog/list");
        request.SetJsonBody(requestModel, _jsonSerializerSetting);

        Worklog[] worklogs;

        var response = await ExecuteAsync(request);
        await response.CheckResponse();
        worklogs = await response.GetData<Worklog[]>();

        return worklogs;
    }

    public async Task<Worklog> CreateWorklog(string issueKey, AddUpdateWorklogRequestModel worklog)
    {
        var request = new JsonRestRequest(HttpMethod.Post, "issue/{issueIdOrKey}/worklog");
        request.SetUrlSegment("issueIdOrKey", issueKey);
        request.SetJsonBody(worklog, _jsonSerializerSetting);
            
        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var addedWorklog = await response.GetData<Worklog>();
        return addedWorklog; 
    }

    public async Task<Worklog> UpdateWorklog(string issueKey, string worklogId, AddUpdateWorklogRequestModel worklog)
    {
        var request = new JsonRestRequest(HttpMethod.Put, "issue/{issueIdOrKey}/worklog/{id}");
        request.SetUrlSegment("issueIdOrKey", issueKey);
        request.SetUrlSegment("id", worklogId);
        request.SetJsonBody(worklog, _jsonSerializerSetting);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var updatedWorklog = await response.GetData<Worklog>();
        return updatedWorklog;
    }

    public async Task DeleteWorklog(string issueKey, string worklogId)
    {
        var request = new JsonRestRequest(HttpMethod.Delete, "issue/{issueIdOrKey}/worklog/{id}");
        request.SetUrlSegment("issueIdOrKey", issueKey);
        request.SetUrlSegment("id", worklogId);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();
    }
        
    public async Task<Issue> GetIssue(string issueKey)
    {
        var request = new JsonRestRequest(HttpMethod.Get, "issue/{issueIdOrKey}");
        request.SetUrlSegment("issueIdOrKey", issueKey);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();
        return await response.GetData<Issue>();
    }
}