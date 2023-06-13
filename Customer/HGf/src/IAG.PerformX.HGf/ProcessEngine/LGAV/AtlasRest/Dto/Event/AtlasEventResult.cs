using System;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;

public class AtlasEventResult
{
    [JsonProperty("HGfLGAVBestaetigung")]
    public DateTime HgfLgavBestaetigung { get; set; }

    [JsonProperty("HGfLGAVID")]
    public int HgfLgavId { get; set; }

    [JsonProperty("HGfLGAVUebermittelt")]
    public bool HgfLgavUebermittelt { get; set; }
}