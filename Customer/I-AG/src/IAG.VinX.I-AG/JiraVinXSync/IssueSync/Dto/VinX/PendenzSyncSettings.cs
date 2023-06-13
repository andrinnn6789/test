using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("PendenzSync_")]
[Table("PendenzSyncGeneralSettings")]
public class PendenzSyncSettings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // ReSharper disable once InconsistentNaming
    public int ID { get; set; }

    public string Name { get; set; }

    public string LastSync { get; set; }

    // ReSharper disable once InconsistentNaming
    public int AdresseID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int SyncUserID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int KostenartID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int KostenstelleID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int PendenzstatusOpenID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int PendenzstatusProgressID { get; set; }

    // ReSharper disable once InconsistentNaming
    public int PendenzstatusDoneID { get; set; }
}