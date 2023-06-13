using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class IssueLink
{
    public string Id { get; set; }

    public LinkType Type { get; set; }

    public InwardIssue InwardIssue { get; set; }
}