using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.Dto.VinX;

[UsedImplicitly]
[TablePrefix("Version_")]
[Table("Version")]
public class Version
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public decimal VersionsNummer { get; set; }
    public string Bezeichnung { get; set; }
    public int KostenartID { get; set; }
    public bool IstEntwicklung { get; set; }
    public decimal ModelNummer { get; set; }
    public DateTime ReleaseDatum { get; set; }
    public string JiraTicket { get; set; }
    public bool SyncToJira { get; set; }
    public bool IsSyncedInJira { get; set; }

}