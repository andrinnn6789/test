using System;

using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.Jira;

[Serializable]
public class CreateComponentRequestModel
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("assigneeType")] public string AssigneeType { get; set; }
    [JsonProperty("project")] public string Project { get; set; }
}