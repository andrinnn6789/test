using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class Issue
{
    public string Id { get; set; }

    public string Key { get; set; }

    public Fields Fields { get; set; }
}