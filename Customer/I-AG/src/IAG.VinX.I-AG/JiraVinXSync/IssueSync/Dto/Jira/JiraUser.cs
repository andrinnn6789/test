using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class JiraUser
{
    public string Name { get; set; }

    public string Key { get; set; }

    public string EmailAddress { get; set; }

    public string DisplayName { get; set; }

    public bool Active { get; set; }
}