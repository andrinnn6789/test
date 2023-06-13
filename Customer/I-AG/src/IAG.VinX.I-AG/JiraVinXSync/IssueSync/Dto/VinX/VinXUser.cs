using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("Mit_")]
[Table("Mitarbeiter")]
public class VinXUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public string URef { get; set; }

    public AtlasBoolean Aktiv { get; set; }

    [NotMapped]
    public bool IsActive
    {
        get => Aktiv;
        set => Aktiv = value;
    }
}