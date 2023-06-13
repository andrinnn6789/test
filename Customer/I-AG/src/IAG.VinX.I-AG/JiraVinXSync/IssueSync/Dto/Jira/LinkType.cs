using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class LinkType
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Inward { get; set; }

    public string Outward { get; set; }
}