using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("PendenzSyncProject_")]
[Table("PendenzSyncProjectSettings")]
public class PendenzSyncProjectSettings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int ID { get; set; }
    public int ProjectSettingType { get; set; }
    public string Name { get; set; }
    public int KostenartID { get; set; }
    public int KostenstelleID { get; set; }
    public bool SyncVersionsToJira { get; set; }
}