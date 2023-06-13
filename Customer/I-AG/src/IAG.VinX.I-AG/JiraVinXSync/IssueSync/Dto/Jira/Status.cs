using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class Status
{
    public string Description { get; set; }

    public string Name { get; set; }

    public string Id { get; set; }

    public StatusCategory StatusCategory { get; set; }
}