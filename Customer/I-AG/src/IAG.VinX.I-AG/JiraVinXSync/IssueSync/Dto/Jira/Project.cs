using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class Project
{
    public string Id { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string ProjectTypeKey { get; set; }

    public ProjectCategory ProjectCategory { get; set; }
}