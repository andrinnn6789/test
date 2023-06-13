using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess;

public class JiraIssueClient : RestClient
{
    private const string Fields =
        @"id,key,assignee,reporter,summary,updated,created,resolutiondate,duedate,issuetype,
            status,project,customfield_10291,aggregatetimeoriginalestimate,customfield_10690,customfield_10092,parent,issuelinks";

    public JiraIssueClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, logger)
    {
    }

    public async Task<List<Issue>> GetIssues(DateTime lastSync)
    {
        var jql = $"updated >='{lastSync:yyyy-MM-dd HH:mm}' order by updated asc";

        var totalUpdated = await GetIssueCount(jql);

        var request = new JsonRestRequest(HttpMethod.Get, "search");
        request.SetQueryParameter("jql", jql);
        request.SetQueryParameter("fields", Fields);
        request.SetQueryParameter("maxResults", $"{totalUpdated}");

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var jiraRootObject = await response.GetData<RootObject>();
        var maxResults = jiraRootObject.MaxResults;
        var jsonIssues = jiraRootObject.Issues;

        if (totalUpdated > maxResults)
        {
            throw new Exception($"Overload. There are more updated issues ({totalUpdated}) than max loadable ({maxResults}).");
        }

        return jsonIssues;
    }

    private async Task<int> GetIssueCount(string jqlString)
    {
        var request = new JsonRestRequest(HttpMethod.Get, "search");
        request.SetQueryParameter("jql", jqlString);
        request.SetQueryParameter("fields", "*none");
        request.SetQueryParameter("maxResults", "0");

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        var jiraRootObject = await response.GetData<RootObject>();
        return jiraRootObject.Total;
    }
}