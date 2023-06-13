using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.Jira;

[UsedImplicitly]
public class StatusCategory
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }
}