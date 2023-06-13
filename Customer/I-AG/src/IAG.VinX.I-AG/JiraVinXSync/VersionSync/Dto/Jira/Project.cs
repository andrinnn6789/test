using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.Jira;

[UsedImplicitly]
public class Project
{
    [JsonProperty("expand")]
    public string Expand { get; set; }

    [JsonProperty("self")]
    public string Self { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("projectTypeKey")]
    public string ProjectTypeKey { get; set; }
}
