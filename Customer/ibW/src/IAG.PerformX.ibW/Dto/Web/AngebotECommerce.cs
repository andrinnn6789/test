using System;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.PerformX.ibW.Dto.Web;

[UsedImplicitly]
[Table("REST3AngebotECommerce")]
public class AngebotECommerce
{
    [JsonIgnore]
    public int AngebotId { get; set; }
    public int ECommerceGruppeId { get; set; }
    [JsonIgnore]
    public DateTime LastChange { get; set; }
}