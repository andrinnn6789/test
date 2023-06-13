using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class Fields
{
    public string Summary { get; set; }

    public IssueType Issuetype { get; set; }

    public Issue Parent { get; set; }

    [JsonProperty("customfield_10291")]
    public decimal? Expenses { get; set; }

    [JsonProperty("customfield_10092")]
    public string EpicLink { get; set; }

    public DateTime? Created { get; set; }

    [JsonProperty("customfield_10690")]
    public List<Organization> Organizations { get; set; }

    public Project Project { get; set; }

    public JiraUser Reporter { get; set; }
        
    public int? Aggregatetimeoriginalestimate { get; set; }

    public DateTime? Duedate { get; set; }

    public DateTime? Resolutiondate { get; set; }

    public List<IssueLink> IssueLinks { get; set; }

    public JiraUser Assignee { get; set; }

    public DateTime Updated { get; set; }

    public Status Status { get; set; }
}