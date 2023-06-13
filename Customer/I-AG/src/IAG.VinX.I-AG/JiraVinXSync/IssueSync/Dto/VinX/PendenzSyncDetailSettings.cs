using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("PendenzSyncProject_")]
[Table("PendenzSyncProjectSettings")]
public class PendenzSyncDetailSettings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public int ProjectSettingType { get; set; }

    public string Name { get; set; }

    // ReSharper disable once InconsistentNaming
    public int KostenartID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int KostenstelleID { get; set; }
}