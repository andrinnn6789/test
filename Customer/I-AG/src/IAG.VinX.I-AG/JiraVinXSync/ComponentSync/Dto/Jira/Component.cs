using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.Jira;

[UsedImplicitly]
public class Component
{
    [JsonProperty("self")]
    public string Self { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("assigneeType")]
    public string AssigneeType { get; set; }
}