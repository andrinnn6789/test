using System.ComponentModel.DataAnnotations.Schema;

using IAG.VinX.BaseData.Dto.Sybase;

using Newtonsoft.Json;

namespace IAG.VinX.Zweifel.S1M.Dto.Sybase;

[Table("Spezialkondition")]
public class SpecialConditionZweifel : SpecialCondition
{
    [JsonIgnore]
    [Column("SpezKond_anAppUebermitteln")]
    public bool SendToApp { get; set; }
}