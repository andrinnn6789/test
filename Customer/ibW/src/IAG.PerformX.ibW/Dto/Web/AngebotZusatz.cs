using System;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.ibW.Dto.Web;

[UsedImplicitly]
[Table("REST3AngebotZusatz")]
public class AngebotZusatz
{
    [JsonIgnore]
    public int AngebotId { get; set; }
    public string Bezeichnung { get; set; }
    public string HtmlDe { get; set; }
    public string HtmlIt { get; set; }
    [JsonIgnore]
    public DateTime LastChange { get; set; }
}