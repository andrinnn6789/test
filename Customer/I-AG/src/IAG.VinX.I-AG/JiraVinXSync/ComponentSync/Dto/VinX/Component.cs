using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("Komp_")]
[Table("Komponente")]
public class Component
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string Name { get; set; }
    public bool IsSyncedInJira { get; set; }
}