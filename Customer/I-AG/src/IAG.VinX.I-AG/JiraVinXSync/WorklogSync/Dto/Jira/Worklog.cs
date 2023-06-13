using System;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.Jira;

[UsedImplicitly]
public class Worklog
{
    public string Id { get; set; }
    public string IssueId { get; set; }
    public string Comment { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime Started { get; set; }
    public string TimeSpent { get; set; }
    public int TimeSpentSeconds { get; set; }
}