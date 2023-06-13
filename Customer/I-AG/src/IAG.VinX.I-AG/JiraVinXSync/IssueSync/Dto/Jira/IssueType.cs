using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class IssueType
{
    public string Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public bool Subtask { get; set; }
}