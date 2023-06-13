using System;

using Newtonsoft.Json;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.Jira;

[Serializable]
public class AddUpdateWorklogRequestModel
{
    [JsonProperty("comment")] public string Comment { get; set; }
    [JsonProperty("started")] public string Started { get; set; }
    [JsonProperty("timeSpentSeconds")] public int TimeSpentSeconds { get; set; }
}