using System.Collections.Generic;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class RootObject
{
    public int StartAt { get; set; }

    public int MaxResults { get; set; }

    public int Total { get; set; }

    public List<Issue> Issues { get; set; }
}