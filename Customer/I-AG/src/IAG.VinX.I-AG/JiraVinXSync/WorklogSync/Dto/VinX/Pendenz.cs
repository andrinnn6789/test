using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("Pendenz_")]
[Table("Pendenz")]
public class Pendenz
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public string JiraKey { get; set; }
}