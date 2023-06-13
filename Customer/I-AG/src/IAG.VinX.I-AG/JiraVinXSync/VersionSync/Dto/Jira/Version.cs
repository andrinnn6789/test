using JetBrains.Annotations;
using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.Jira;

[UsedImplicitly]
public class Version
{
    [JsonProperty("self")]
    public string Self { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("archived")]
    public bool Archived { get; set; }

    [JsonProperty("released")] 
    public bool Released { get; set; }

    [JsonProperty("projectId")]
    public int ProjectId { get; set; }
}