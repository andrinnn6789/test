using System;

using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.Jira;

[Serializable]
public class CreateVersionRequestModel
{
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("releaseDate")] public string ReleaseDate { get; set; }
    [JsonProperty("project")] public string Project { get; set; }
}